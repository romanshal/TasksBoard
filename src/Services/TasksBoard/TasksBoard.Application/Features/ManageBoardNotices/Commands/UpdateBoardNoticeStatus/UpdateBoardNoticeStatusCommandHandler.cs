﻿using Common.Blocks.Interfaces.Services;
using Common.Blocks.Models.DomainResults;
using EventBus.Messages.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

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
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                return Result.Failure<Guid>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardNotice = await _unitOfWork.GetRepository<BoardNotice>().GetAsync(request.NoticeId, cancellationToken);
            if (boardNotice is null)
            {
                _logger.LogWarning("Board notice with id '{noticeId}' was not found.", request.NoticeId);
                return Result.Failure<Guid>(BoardNoticeErrors.NotFound);

                //throw new NotFoundException($"Board notice with id '{request.NoticeId}' not found.");
            }

            boardNotice.Completed = request.Complete;

            _unitOfWork.GetRepository<BoardNotice>().Update(boardNotice);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0 || boardNotice.Id == Guid.Empty)
            {
                _logger.LogError("Can't update board notice status with id '{boardNoticeId}'.", boardNotice.Id);
                return Result.Failure<Guid>(BoardNoticeErrors.CantUpdate);

                //throw new ArgumentException(nameof(boardNotice));
            }

            await _outboxService.CreateNewOutboxEvent(new UpdateNoticeStatusEvent
            {
                BoardId = board.Id,
                BoardName = board.Name,
                NoticeId = boardNotice.Id,
                AccountId = request.AccountId,
                AccountName = request.AccountName,
                Completed = request.Complete,
                BoardMembersIds = [.. board.BoardMembers.Where(member => member.AccountId != request.AccountId).Select(member => member.AccountId)]
            }, cancellationToken);

            _logger.LogInformation("Board notice with id '{id}' updated in board with id '{boardId}'.", boardNotice.Id, request.BoardId);

            return Result.Success(boardNotice.Id);
        }
    }
}
