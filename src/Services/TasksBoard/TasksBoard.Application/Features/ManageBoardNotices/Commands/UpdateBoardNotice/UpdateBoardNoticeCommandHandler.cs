using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using Common.Outbox.Extensions;
using Common.Outbox.Interfaces.Factories;
using EventBus.Messages.Abstraction.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNotice
{
    public class UpdateBoardNoticeCommandHandler(
        ILogger<UpdateBoardNoticeCommandHandler> logger,
        IOutboxEventFactory outboxFactory,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateBoardNoticeCommand, Result<Guid>>
    {
        private readonly ILogger<UpdateBoardNoticeCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxEventFactory _outboxFactory = outboxFactory;

        public async Task<Result<Guid>> Handle(UpdateBoardNoticeCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetBoardRepository().GetAsync(BoardId.Of(request.BoardId), noTracking: true, include: true, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                    return Result.Failure<Guid>(BoardErrors.NotFound);
                }

                var boardNotice = await _unitOfWork.GetBoardNoticeRepository().GetAsync(BoardNoticeId.Of(request.NoticeId), token);
                if (boardNotice is null)
                {
                    _logger.LogWarning("Board notice with id '{noticeId}' was not found.", request.NoticeId);
                    return Result.Failure<Guid>(BoardNoticeErrors.NotFound);
                }

                boardNotice.Definition = request.Definition;
                boardNotice.BackgroundColor = request.BackgroundColor;

                _unitOfWork.GetBoardNoticeRepository().Update(boardNotice);

                var outboxEvent = _outboxFactory.Create(new UpdateNoticeEvent
                {
                    BoardId = board.Id.Value,
                    BoardName = board.Name,
                    AccountId = request.AccountId,
                    NoticeId = boardNotice.Id.Value,
                    NoticeDefinition = boardNotice.Definition,
                    UsersInterested = [.. board.BoardMembers.Where(m => m.AccountId != AccountId.Of(request.AccountId)).Select(m => m.AccountId.Value)]
                });

                _unitOfWork.GetOutboxEventRepository().Add(outboxEvent);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0 || boardNotice.Id.Value == Guid.Empty)
                {
                    _logger.LogError("Can't update board notice with id '{boardNoticeId}'.", boardNotice.Id);
                    return Result.Failure<Guid>(BoardNoticeErrors.CantUpdate);
                }

                _logger.LogInformation("Board notice with id '{id}' updated in board with id '{boardId}'.", boardNotice.Id, request.BoardId);

                return Result.Success(boardNotice.Id.Value);
            }, cancellationToken);
        }
    }
}
