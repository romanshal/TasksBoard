using Common.Blocks.Interfaces.Services;
using Common.Blocks.Models.DomainResults;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNotice
{
    public class UpdateBoardNoticeCommandHandler(
        ILogger<UpdateBoardNoticeCommandHandler> logger,
        IOutboxService outboxService,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateBoardNoticeCommand, Result<Guid>>
    {
        private readonly ILogger<UpdateBoardNoticeCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxService _outboxService = outboxService;

        public async Task<Result<Guid>> Handle(UpdateBoardNoticeCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                    return Result.Failure<Guid>(BoardErrors.NotFound);

                    //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
                }

                var boardNotice = await _unitOfWork.GetRepository<BoardNotice>().GetAsync(request.NoticeId, token);
                if (boardNotice is null)
                {
                    _logger.LogWarning("Board notice with id '{noticeId}' was not found.", request.NoticeId);
                    return Result.Failure<Guid>(BoardNoticeErrors.NotFound);

                    //throw new NotFoundException($"Board notice with id '{request.NoticeId}' not found.");
                }

                boardNotice.Definition = request.Definition;
                boardNotice.BackgroundColor = request.BackgroundColor;

                _unitOfWork.GetRepository<BoardNotice>().Update(boardNotice);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0 || boardNotice.Id == Guid.Empty)
                {
                    _logger.LogError("Can't update board notice with id '{boardNoticeId}'.", boardNotice.Id);
                    return Result.Failure<Guid>(BoardNoticeErrors.CantUpdate);

                    //throw new ArgumentException(nameof(boardNotice));
                }

                await _outboxService.CreateNewOutboxEvent(new UpdateNoticeEvent
                {
                    BoardId = board.Id,
                    BoardName = board.Name,
                    AccountId = request.AccountId,
                    AccountName = board.BoardMembers.First(m => m.AccountId == request.AccountId).Nickname,
                    NoticeId = boardNotice.Id,
                    NoticeDefinition = boardNotice.Definition,
                    BoardMembersIds = [.. board.BoardMembers.Where(m => m.AccountId != request.AccountId).Select(m => m.AccountId)]
                }, token);

                _logger.LogInformation("Board notice with id '{id}' updated in board with id '{boardId}'.", boardNotice.Id, request.BoardId);

                return Result.Success(boardNotice.Id);
            }, cancellationToken);
        }
    }
}
