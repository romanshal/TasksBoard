using Common.Blocks.Exceptions;
using Common.Blocks.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember
{
    public class DeleteBoardMemberCommandHandler(
        ILogger<DeleteBoardMemberCommandHandler> logger,
        IOutboxService outboxService,
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteBoardMemberCommand, Unit>
    {
        private readonly ILogger<DeleteBoardMemberCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxService _outboxService = outboxService;

        public async Task<Unit> Handle(DeleteBoardMemberCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var member = board.BoardMembers.FirstOrDefault(member => member.Id == request.MemberId);
            if (member is null)
            {
                _logger.LogWarning($"Board member with id '{request.MemberId}' not found in board '{request.BoardId}'.");
                throw new NotFoundException($"Board member with id '{request.MemberId}' not found in board '{request.BoardId}'.");
            }

            await _unitOfWork.GetBoardMemberRepository().Delete(member, true, cancellationToken);

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

            await _outboxService.CreateNewOutboxEvent(removeEvent, cancellationToken);

            _logger.LogInformation($"Board member with account id '{member.AccountId}' deleted from boar with id '{request.BoardId}'.");

            return Unit.Value;
        }
    }
}
