using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard
{
    public class UpdateBoardCommandHandler(
        ILogger<GetPaginatedPublicBoardsQueryHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateBoardCommand, Guid>
    {
        private readonly ILogger<GetPaginatedPublicBoardsQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            board.Name = request.Name;
            board.Description = request.Description;
            board.Public = request.Public;

            if (request.Image is not null)
            {
                if (board.BoardImage is null)
                {
                    board.BoardImage = new BoardImage
                    {
                        Image = request.Image,
                        Extension = request.ImageExtension!
                    };
                }
                else
                {
                    board.BoardImage.Extension = request.ImageExtension!;
                    board.BoardImage.Image = request.Image;
                }
            }
            else
            {
                if (board.BoardImage is not null)
                {
                    board.BoardImage = null;
                }
            }

            board.Tags.Clear();
            board.Tags = [.. request.Tags.Select(tag => new BoardTag { Tag = tag })];

            await _unitOfWork.GetRepository<Board>().Update(board, true, cancellationToken);

            if (board.Id == Guid.Empty)
            {
                _logger.LogError("Can't update board with id '{id}'.", board.Id);
                throw new ArgumentException(nameof(board));
            }

            _logger.LogInformation("Board with id '{id}' updated'.", board.Id);

            return board.Id;
        }
    }
}
