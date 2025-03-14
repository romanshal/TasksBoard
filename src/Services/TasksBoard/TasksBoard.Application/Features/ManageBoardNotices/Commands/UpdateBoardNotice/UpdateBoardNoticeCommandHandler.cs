using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNotice
{
    public class UpdateBoardNoticeCommandHandler(
        ILogger<GetBoardByIdQueryHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateBoardNoticeCommand, Guid>
    {
        private readonly ILogger<GetBoardByIdQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(UpdateBoardNoticeCommand request, CancellationToken cancellationToken)
        {
            var boardNotice = await _unitOfWork.GetRepository<BoardNotice>().GetAsync(request.Id, cancellationToken);
            if (boardNotice is null)
            {
                _logger.LogWarning($"Board notice with id '{request.Id}' not found.");
                throw new NotFoundException($"Board notice with id '{request.Id}' not found.");
            }

            boardNotice.Definition = request.Definition;
            boardNotice.NoticeStatusId = request.NoticeStatusId;

            await _unitOfWork.GetRepository<BoardNotice>().Update(boardNotice, true, cancellationToken);

            if (boardNotice.Id == Guid.Empty)
            {
                _logger.LogError("Can't update board notice.");
                throw new ArgumentException(nameof(boardNotice));
            }

            return boardNotice.Id;
        }
    }
}
