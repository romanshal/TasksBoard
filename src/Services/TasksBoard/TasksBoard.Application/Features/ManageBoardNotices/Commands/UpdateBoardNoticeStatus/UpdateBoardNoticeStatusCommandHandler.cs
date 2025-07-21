
using Common.Blocks.Exceptions;
using Common.Blocks.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNoticeStatus
{
    public class UpdateBoardNoticeStatusCommandHandler(
        ILogger<GetPaginatedPublicBoardsQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IOutboxService outboxService) : IRequestHandler<UpdateBoardNoticeStatusCommand, Guid>
    {
        private readonly ILogger<GetPaginatedPublicBoardsQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxService _outboxService = outboxService;

        public async Task<Guid> Handle(UpdateBoardNoticeStatusCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardNotice = await _unitOfWork.GetRepository<BoardNotice>().GetAsync(request.NoticeId, cancellationToken);
            if (boardNotice is null)
            {
                _logger.LogWarning("Board notice with id '{noticeId}' not found.", request.NoticeId);
                throw new NotFoundException($"Board notice with id '{request.NoticeId}' not found.");
            }

            boardNotice.Completed = request.Complete;

            await _unitOfWork.GetRepository<BoardNotice>().Update(boardNotice, true, cancellationToken);

            if (boardNotice.Id == Guid.Empty)
            {
                _logger.LogError("Can't update board notice.");
                throw new ArgumentException(nameof(boardNotice));
            }

            await _outboxService.CreateNewOutboxEvent(new UpdateNoticeStatusEvent
            {
                BoardId = board.Id,
                BoardName = board.Name,
                NoticeId = boardNotice.Id,
                AccountId = request.AccountId,
                AccountName = request.AccountName,
                Completed = request.Complete,
                BoardMembersIds = [.. board.BoardMembers.Where(member => member.AccountId != request.AccountId).Select(member => member.AccountId)]
            }, cancellationToken);

            _logger.LogInformation("Board notice with id '{id}' updated in board with id '{boardId}'.", boardNotice.Id, request.BoardId);

            return boardNotice.Id;
        }
    }
}
