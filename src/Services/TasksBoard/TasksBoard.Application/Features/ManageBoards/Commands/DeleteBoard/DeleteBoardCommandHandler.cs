using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Interfaces.Services;
using EventBus.Messages.Events;
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
        IOutboxService outboxService) : IRequestHandler<DeleteBoardCommand, Result>
    {
        private readonly ILogger<DeleteBoardCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxService _outboxService = outboxService;

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

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0)
                {
                    _logger.LogError("Can't delete board with id '{boardId}'.", request.Id);
                    return Result.Failure(BoardErrors.CantDelete);
                }

                await _outboxService.CreateNewOutboxEvent(new DeleteBoardEvent
                {
                    BoardId = board.Id.Value,
                    BoardName = board.Name,
                    AccountId = request.AccountId,
                    BoardMembersIds = [.. board.BoardMembers.Where(m => m.AccountId != AccountId.Of(request.AccountId)).Select(m => m.AccountId.Value)]
                }, token);

                _logger.LogInformation("Board with id '{id}' deleted'.", request.Id);

                return Result.Success();
            }, cancellationToken);
        }
    }
}
