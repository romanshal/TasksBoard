using AutoMapper;
using Common.Blocks.Exceptons;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.Boards.Commands.UpdateBoard
{
    public class UpdateBoardCommandHandler(
        ILogger<GetBoardByIdQueryHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateBoardCommand, Guid>
    {
        private readonly ILogger<GetBoardByIdQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.Id, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.Id}' not found.");
                throw new NotFoundException<Board>($"Board with id '{request.Id}' not found.");
            }

            board.Name = request.Name;
            board.Description = request.Description;

            await _unitOfWork.GetRepository<Board>().Update(board, true, cancellationToken);

            if (board.Id == Guid.Empty)
            {
                _logger.LogError("Can't update board.");
                throw new ArgumentException(nameof(board));
            }

            return board.Id;
        }
    }
}
