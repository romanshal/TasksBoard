using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Abstraction.Interfaces.Factories;
using Common.Outbox.Extensions;
using EventBus.Messages.Abstraction.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard
{
    public class DeleteBoardCommandHandler(
        ILogger<DeleteBoardCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IOutboxEventFactory outboxFactory) : IRequestHandler<DeleteBoardCommand, Result>
    {
        private readonly ILogger<DeleteBoardCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxEventFactory _outboxFactory = outboxFactory;

        public async Task<Result> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetBoardRepository().GetAsync(BoardId.Of(request.Id), noTracking: true, include: true, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{id}' was not found.", request.Id);
                    return Result.Failure(BoardErrors.NotFound);
                }

                _unitOfWork.GetBoardRepository().Delete(board);

                var outboxEvent = _outboxFactory.Create(new DeleteBoardEvent
                {
                    BoardId = board.Id.Value,
                    BoardName = board.Name,
                    AccountId = request.AccountId,
                    UsersInterested = [.. board.BoardMembers.Where(m => m.AccountId != AccountId.Of(request.AccountId)).Select(m => m.AccountId.Value)]
                });

                _unitOfWork.GetOutboxEventRepository().Add(outboxEvent);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0)
                {
                    _logger.LogError("Can't delete board with id '{boardId}'.", request.Id);
                    return Result.Failure(BoardErrors.CantDelete);
                }

                _logger.LogInformation("Board with id '{id}' deleted'.", request.Id);

                return Result.Success();
            }, cancellationToken);
        }
    }
}
