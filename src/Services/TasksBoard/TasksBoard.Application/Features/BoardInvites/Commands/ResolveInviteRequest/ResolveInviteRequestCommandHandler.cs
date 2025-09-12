using Common.Blocks.Constants;
using Common.Blocks.Models.DomainResults;
using Common.Outbox.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.BoardInvites.Commands.ResolveInviteRequest
{
    public class ResolveInviteRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxService outboxService,
        IMediator mediator,
        ILogger<ResolveInviteRequestCommandHandler> logger) : IRequestHandler<ResolveInviteRequestCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;
        private readonly IOutboxService _outboxService = outboxService;
        private readonly ILogger<ResolveInviteRequestCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(ResolveInviteRequestCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetRepository<Board, BoardId>().GetAsync(BoardId.Of(request.BoardId), token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                    return Result.Failure<Guid>(BoardErrors.NotFound);
                }

                var inviteRequest = await _unitOfWork.GetRepository<BoardInviteRequest, BoardInviteId>().GetAsync(BoardInviteId.Of(request.RequestId), token);
                if (inviteRequest is null)
                {
                    _logger.LogWarning("Board invite request with id '{requestId}' was not found.", request.RequestId);
                    return Result.Failure<Guid>(BoardInviteErrors.NotFound);
                }

                inviteRequest.Status = request.Decision ? (int)BoardInviteRequestStatuses.Accepted : (int)BoardInviteRequestStatuses.Rejected;

                _unitOfWork.GetRepository<BoardInviteRequest, BoardInviteId>().Update(inviteRequest);

                if (request.Decision)
                {
                    var result = await _mediator.Send(new AddBoardMemberCommand
                    {
                        BoardId = inviteRequest.BoardId.Value,
                        AccountId = inviteRequest.ToAccountId
                    }, token);

                    if (result.IsFailure)
                    {
                        _logger.LogError("Can't add new board member.");
                        return Result.Failure<Guid>(BoardMemberErrors.CantCreate);
                    }

                    await _outboxService.CreateNewOutboxEvent(new NewBoardMemberEvent
                    {
                        BoardId = board.Id.Value,
                        BoardName = board.Name,
                        AccountId = inviteRequest.ToAccountId,
                        BoardMembersIds = [.. board.BoardMembers.Where(member => member.AccountId != inviteRequest.ToAccountId).Select(member => member.AccountId)]
                    }, token);
                }
                else
                {
                    await _unitOfWork.SaveChangesAsync(token);
                }

                return Result.Success(inviteRequest.Id.Value);
            }, cancellationToken);
        }
    }
}
