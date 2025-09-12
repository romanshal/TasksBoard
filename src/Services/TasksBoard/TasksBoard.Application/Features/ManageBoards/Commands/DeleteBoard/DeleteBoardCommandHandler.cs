using Common.Blocks.Models.DomainResults;
using Common.Outbox.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard
{
    public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand, Result>
    {
        private readonly ILogger<DeleteBoardCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxService _outboxService;
        private readonly IBoardRepository _boardRepository;

        public DeleteBoardCommandHandler(
            ILogger<DeleteBoardCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IOutboxService outboxService)
        {
            this._logger = logger;
            this._unitOfWork = unitOfWork;
            this._outboxService = outboxService;
            this._boardRepository = _unitOfWork.GetBoardRepository();
        }

        public async Task<Result> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _boardRepository.GetAsync(BoardId.Of(request.Id), token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{id}' was not found.", request.Id);
                    return Result.Failure(BoardErrors.NotFound);
                }

                _boardRepository.Delete(board);

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
                    BoardMembersIds = [.. board.BoardMembers.Where(m => m.AccountId != request.AccountId).Select(m => m.AccountId)]
                }, token);

                _logger.LogInformation("Board with id '{id}' deleted'.", request.Id);

                return Result.Success();
            }, cancellationToken);
        }
    }
}
