using Common.Blocks.Models.DomainResults;
using Common.Outbox.Interfaces.Factories;
using Common.Outbox.Extensions;
using EventBus.Messages.Abstraction.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Constants.Statuses;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.BoardInvites.Commands.ResolveInviteRequest
{
    public class ResolveInviteRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxEventFactory outboxFactory,
        IMediator mediator,
        ILogger<ResolveInviteRequestCommandHandler> logger) : IRequestHandler<ResolveInviteRequestCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;
        private readonly IOutboxEventFactory _outboxFactory = outboxFactory;
        private readonly ILogger<ResolveInviteRequestCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(ResolveInviteRequestCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetBoardRepository().GetAsync(BoardId.Of(request.BoardId), noTracking: true, include: true, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                    return Result.Failure<Guid>(BoardErrors.NotFound);
                }

                var inviteRequest = await _unitOfWork.GetBoardInviteRequestRepository().GetAsync(BoardInviteId.Of(request.RequestId), token);
                if (inviteRequest is null)
                {
                    _logger.LogWarning("Board invite request with id '{requestId}' was not found.", request.RequestId);
                    return Result.Failure<Guid>(BoardInviteErrors.NotFound);
                }

                inviteRequest.Status = request.Decision ? (int)BoardInviteRequestStatuses.Accepted : (int)BoardInviteRequestStatuses.Rejected;

                _unitOfWork.GetBoardInviteRequestRepository().Update(inviteRequest);

                if (request.Decision)
                {
                    var result = await _mediator.Send(new AddBoardMemberCommand
                    {
                        BoardId = inviteRequest.BoardId.Value,
                        AccountId = inviteRequest.ToAccountId.Value
                    }, token);

                    if (result.IsFailure)
                    {
                        _logger.LogError("Can't add new board member.");
                        return Result.Failure<Guid>(BoardMemberErrors.CantCreate);
                    }

                    var outboxEvent = _outboxFactory.Create(new NewBoardMemberEvent
                    {
                        BoardId = board.Id.Value,
                        BoardName = board.Name,
                        AccountId = inviteRequest.ToAccountId.Value,
                        UsersInterested = [.. board.BoardMembers.Where(member => member.AccountId != inviteRequest.ToAccountId).Select(member => member.AccountId.Value)]
                    });

                    _unitOfWork.GetOutboxEventRepository().Add(outboxEvent);
                }

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0)
                {
                    _logger.LogError("Can't create new invite response to board with id '{boardId}'.", request.BoardId);
                    return Result.Failure<Guid>(BoardInviteErrors.CantCreate(board.Name));
                }

                return Result.Success(inviteRequest.Id.Value);
            }, cancellationToken);
        }
    }
}
