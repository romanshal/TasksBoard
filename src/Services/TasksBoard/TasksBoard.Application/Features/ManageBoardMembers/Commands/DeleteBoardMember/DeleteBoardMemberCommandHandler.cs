using Common.Blocks.Interfaces.Services;
using Common.Blocks.Models.DomainResults;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember
{
    public class DeleteBoardMemberCommandHandler(
        ILogger<DeleteBoardMemberCommandHandler> logger,
        IOutboxService outboxService,
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteBoardMemberCommand, Result>
    {
        private readonly ILogger<DeleteBoardMemberCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxService _outboxService = outboxService;

        public async Task<Result> Handle(DeleteBoardMemberCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                    return Result.Failure(BoardErrors.NotFound);

                    //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
                }

                var member = board.BoardMembers.FirstOrDefault(member => member.Id == request.MemberId);
                if (member is null)
                {
                    _logger.LogWarning("Board member with id '{memberId}' not found in board '{boardId}'.", request.MemberId, request.BoardId);
                    return Result.Failure(BoardMemberErrors.NotFound);

                    //throw new NotFoundException($"Board member with id '{request.MemberId}' not found in board '{request.BoardId}'.");
                }

                _unitOfWork.GetBoardMemberRepository().Delete(member);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0)
                {
                    _logger.LogError("Can't delete board member with id '{memberId}' from board with id '{boardId}'.", request.MemberId, request.BoardId);
                    return Result.Failure(BoardMemberErrors.CantDelete);

                    //throw new ArgumentException($"Can't delete board member with id '{request.MemberId}' from board with id '{request.BoardId}'.");
                }

                var removeEvent = new RemoveBoardMemberEvent
                {
                    BoardId = board.Id,
                    BoardName = board.Name,
                    RemovedAccountId = member.AccountId,
                    RemovedAccountName = member.Nickname,
                    RemoveByAccountId = request.RemoveByUserId,
                    RemoveByAccountName = board.BoardMembers.FirstOrDefault(member => member.AccountId == request.RemoveByUserId)!.Nickname,
                    BoardMembersIds = [.. board.BoardMembers.Where(member => member.AccountId != request.RemoveByUserId).Select(member => member.AccountId)]
                };

                removeEvent.BoardMembersIds.Add(member.AccountId);

                await _outboxService.CreateNewOutboxEvent(removeEvent, token);

                _logger.LogInformation("Board member with account id '{accountId}' deleted from boar with id '{boardId}'.", member.AccountId, request.BoardId);

                return Result.Success();
            }, cancellationToken);
        }
    }
}
