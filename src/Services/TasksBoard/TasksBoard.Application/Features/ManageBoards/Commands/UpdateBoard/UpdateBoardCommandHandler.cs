using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard
{
    public class UpdateBoardCommandHandler(
        ILogger<GetBoardByIdQueryHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateBoardCommand, Guid>
    {
        private readonly ILogger<GetBoardByIdQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            board.Name = request.Name;
            board.Description = request.Description;

            board.Tags.Clear();
            board.Tags = [.. request.Tags.Select(tag => new BoardTag { Tag = tag })];

            await _unitOfWork.GetRepository<Board>().Update(board, true, cancellationToken);

            if (board.Id == Guid.Empty)
            {
                _logger.LogError($"Can't update board with id '{board.Id}'.");
                throw new ArgumentException(nameof(board));
            }

            _logger.LogInformation($"Board with id '{board.Id}' updated'.");

            return board.Id;
        }
    }
}
