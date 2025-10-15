using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Extensions;
using Common.Outbox.Interfaces.Factories;
using EventBus.Messages.Abstraction.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember
{
    public class DeleteBoardMemberCommandHandler(
        ILogger<DeleteBoardMemberCommandHandler> logger,
        IOutboxEventFactory outboxFactory,
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteBoardMemberCommand, Result>
    {
        private readonly ILogger<DeleteBoardMemberCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxEventFactory _outboxFactory = outboxFactory;

        public async Task<Result> Handle(DeleteBoardMemberCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetBoardRepository().GetAsync(BoardId.Of(request.BoardId), noTracking: true, include: true, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                    return Result.Failure(BoardErrors.NotFound);
                }

                var member = board.BoardMembers.FirstOrDefault(member => member.Id.Value == request.MemberId);
                if (member is null)
                {
                    _logger.LogWarning("Board member with id '{memberId}' not found in board '{boardId}'.", request.MemberId, request.BoardId);
                    return Result.Failure(BoardMemberErrors.NotFound);
                }

                _unitOfWork.GetBoardMemberRepository().Delete(member);

                var outboxEvent = _outboxFactory.Create(new RemoveBoardMemberEvent
                {
                    BoardId = board.Id.Value,
                    BoardName = board.Name,
                    RemovedAccountId = member.AccountId.Value,
                    RemoveByAccountId = request.RemoveByUserId,
                    UsersInterested = [.. board.BoardMembers
                    .Where(member => member.AccountId != AccountId.Of(request.RemoveByUserId))
                    .Select(member => member.AccountId.Value)
                    .Concat([member.AccountId.Value])]
                });

                _unitOfWork.GetOutboxEventRepository().Add(outboxEvent);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0)
                {
                    _logger.LogError("Can't delete board member with id '{memberId}' from board with id '{boardId}'.", request.MemberId, request.BoardId);
                    return Result.Failure(BoardMemberErrors.CantDelete);
                }

                _logger.LogInformation("Board member with account id '{accountId}' deleted from boar with id '{boardId}'.", member.AccountId, request.BoardId);

                return Result.Success();
            }, cancellationToken);
        }
    }
}
