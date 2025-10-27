using Common.Blocks.Models.DomainResults;
using Common.Outbox.Abstraction.Interfaces.Factories;
using Common.Outbox.Extensions;
using EventBus.Messages.Abstraction.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Constants.Statuses;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoardAccesses.Commands.ResolveAccessRequest
{
    public class ResolveAccessRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxEventFactory outboxFactory,
        IMediator mediator,
        ILogger<ResolveAccessRequestCommandHandler> logger) : IRequestHandler<ResolveAccessRequestCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;
        private readonly IOutboxEventFactory _outboxFactory = outboxFactory;
        private readonly ILogger<ResolveAccessRequestCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(ResolveAccessRequestCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetBoardRepository().GetAsync(BoardId.Of(request.BoardId), noTracking: true, include: true, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                    return Result.Failure<Guid>(BoardErrors.NotFound);
                }

                var accessRequest = await _unitOfWork.GetBoardAccessRequestRepository().GetAsync(BoardAccessId.Of(request.RequestId), token);
                if (accessRequest is null)
                {
                    _logger.LogWarning("Board access request with id '{requestId}' was not found.", request.RequestId);
                    return Result.Failure<Guid>(BoardAccessErrors.NotFound);
                }

                accessRequest.Status = request.Decision ? (int)BoardAccessRequestStatuses.Accepted : (int)BoardAccessRequestStatuses.Rejected;

                _unitOfWork.GetBoardAccessRequestRepository().Update(accessRequest);

                if (request.Decision)
                {
                    var result = await _mediator.Send(new AddBoardMemberCommand
                    {
                        BoardId = accessRequest.BoardId.Value,
                        AccountId = accessRequest.AccountId.Value
                    }, cancellationToken);

                    if (result.IsFailure)
                    {
                        _logger.LogError("Can't add new board member.");
                        return Result.Failure<Guid>(result.Error);
                    }

                    var outboxMemberEvent = _outboxFactory.Create(new NewBoardMemberEvent
                    {
                        BoardId = board.Id.Value,
                        BoardName = board.Name,
                        AccountId = accessRequest.AccountId.Value,
                        UsersInterested = [.. board.BoardMembers.Where(member => member.AccountId != accessRequest.AccountId).Select(member => member.AccountId.Value)]
                    });

                    _unitOfWork.GetOutboxEventRepository().Add(outboxMemberEvent);
                }

                var outboxResolveEvent = _outboxFactory.Create(new ResolveAccessRequestEvent
                {
                    BoardId = board.Id.Value,
                    BoardName = board.Name,
                    AccountId = accessRequest.AccountId.Value,
                    SourceAccountId = request.ResolveUserId,
                    Status = request.Decision,
                    UsersInterested = [accessRequest.AccountId.Value]
                });

                _unitOfWork.GetOutboxEventRepository().Add(outboxResolveEvent);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0)
                {
                    _logger.LogError("Can't resolve access request.");
                    return Result.Failure<Guid>(BoardAccessErrors.CantCancel);
                }

                return Result.Success(accessRequest.Id.Value);
            }, cancellationToken);
        }
    }
}
