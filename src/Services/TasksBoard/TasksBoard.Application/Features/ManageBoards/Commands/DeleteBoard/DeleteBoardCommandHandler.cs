using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard
{
    public class DeleteBoardCommandHandler(
        ILogger<GetPaginatedPublicBoardsQueryHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteBoardCommand, Unit>
    {
        private readonly ILogger<GetPaginatedPublicBoardsQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.Id, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.Id}' not found.");
                throw new NotFoundException($"Board with id '{request.Id}' not found.");
            }

            await _unitOfWork.GetRepository<Board>().Delete(board, true, cancellationToken);

            _logger.LogInformation($"Board with id '{board.Id}' deleted'.");

            return Unit.Value;
        }
    }
}
