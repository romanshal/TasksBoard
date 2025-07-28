using Common.Blocks.Exceptions;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard
{
    public class DeleteBoardCommandHandler(
        ILogger<DeleteBoardCommandHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteBoardCommand, Result>
    {
        private readonly ILogger<DeleteBoardCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.Id, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{id}' was not found.", request.Id);
                return Result.Failure(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.Id}' not found.");
            }

            _unitOfWork.GetRepository<Board>().Delete(board);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0)
            {
                _logger.LogError("Can't delete board with id '{boardId}'.", request.Id);
                return Result.Failure(BoardErrors.CantDelete);

                //throw new ArgumentException($"Can't delete board with id '{request.Id}'.");
            }

            _logger.LogInformation("Board with id '{id}' deleted'.", request.Id);

            return Result.Success();
        }
    }
}
