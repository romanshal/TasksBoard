using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardCommand
{
    public class DeleteBoardNoticeCommandHandler(
        ILogger<DeleteBoardNoticeCommandHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteBoardNoticeCommand, Result>
    {
        private readonly ILogger<DeleteBoardNoticeCommandHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> Handle(DeleteBoardNoticeCommand request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                return Result.Failure(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardNotice = await _unitOfWork.GetRepository<BoardNotice>().GetAsync(request.NoticeId, cancellationToken);
            if (boardNotice is null)
            {
                _logger.LogWarning("Board notice with id '{noticeId}' was not found.", request.NoticeId);
                return Result.Failure(BoardNoticeErrors.NotFound);

                //throw new NotFoundException($"Board notice with id '{request.NoticeId}' not found.");
            }

            _unitOfWork.GetRepository<BoardNotice>().Delete(boardNotice);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0)
            {
                _logger.LogError("Can't delete board notice with id '{noticeId}' from board with id '{boardId}'.", request.NoticeId, request.BoardId);
                return Result.Failure(BoardNoticeErrors.CantDelete);

                //throw new ArgumentException($"Can't delete board notice with id '{request.NoticeId}' from board with id '{request.BoardId}'.");
            }

            _logger.LogInformation("Board notice with id '{id}' deleted from board with id '{boardId}'.", boardNotice.Id, request.BoardId);

            return Result.Success();
        }
    }
}
