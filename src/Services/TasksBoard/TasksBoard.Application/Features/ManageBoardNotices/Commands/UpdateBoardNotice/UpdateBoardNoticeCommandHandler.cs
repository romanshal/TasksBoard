using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNotice
{
    public class UpdateBoardNoticeCommandHandler(
        ILogger<GetPaginatedPublicBoardsQueryHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateBoardNoticeCommand, Guid>
    {
        private readonly ILogger<GetPaginatedPublicBoardsQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(UpdateBoardNoticeCommand request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardNotice = await _unitOfWork.GetRepository<BoardNotice>().GetAsync(request.NoticeId, cancellationToken);
            if (boardNotice is null)
            {
                _logger.LogWarning($"Board notice with id '{request.NoticeId}' not found.");
                throw new NotFoundException($"Board notice with id '{request.NoticeId}' not found.");
            }

            boardNotice.Definition = request.Definition;
            boardNotice.BackgroundColor = request.BackgroundColor;

            await _unitOfWork.GetRepository<BoardNotice>().Update(boardNotice, true, cancellationToken);

            if (boardNotice.Id == Guid.Empty)
            {
                _logger.LogError("Can't update board notice.");
                throw new ArgumentException(nameof(boardNotice));
            }

            _logger.LogInformation($"Board notice with id '{boardNotice.Id}' updated in board with id '{request.BoardId}'.");

            return boardNotice.Id;
        }
    }
}
