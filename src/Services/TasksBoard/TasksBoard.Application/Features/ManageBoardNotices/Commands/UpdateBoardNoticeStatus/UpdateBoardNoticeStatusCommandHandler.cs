using Common.Blocks.Models.DomainResults;
using Common.Outbox.Interfaces.Services;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNoticeStatus
{
    public class UpdateBoardNoticeStatusCommandHandler(
        ILogger<UpdateBoardNoticeStatusCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IOutboxService outboxService) : IRequestHandler<UpdateBoardNoticeStatusCommand, Result<Guid>>
    {
        private readonly ILogger<UpdateBoardNoticeStatusCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxService _outboxService = outboxService;

        public async Task<Result<Guid>> Handle(UpdateBoardNoticeStatusCommand request, CancellationToken cancellationToken)
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

                boardNotice.Completed = request.Complete;

                _unitOfWork.GetBoardNoticeRepository().Update(boardNotice);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0 || boardNotice.Id.Value == Guid.Empty)
                {
                    _logger.LogError("Can't update board notice status with id '{boardNoticeId}'.", boardNotice.Id);
                    return Result.Failure<Guid>(BoardNoticeErrors.CantUpdate);
                }

                await _outboxService.CreateNewOutboxEvent(new UpdateNoticeStatusEvent
                {
                    BoardId = board.Id.Value,
                    BoardName = board.Name,
                    NoticeId = boardNotice.Id.Value,
                    AccountId = request.AccountId,
                    Completed = request.Complete,
                    BoardMembersIds = [.. board.BoardMembers.Where(member => member.AccountId != request.AccountId).Select(member => member.AccountId)]
                }, token);

                _logger.LogInformation("Board notice with id '{id}' updated in board with id '{boardId}'.", boardNotice.Id, request.BoardId);

                return Result.Success(boardNotice.Id.Value);
            }, cancellationToken);
        }
    }
}
