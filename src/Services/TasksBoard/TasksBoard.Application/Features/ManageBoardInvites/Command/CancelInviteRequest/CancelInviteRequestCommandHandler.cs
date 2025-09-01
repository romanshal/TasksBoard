using Common.Blocks.Constants;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardInvites.Command.CancelInviteRequest
{
    public class CancelInviteRequestCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CancelInviteRequestCommandHandler> logger) : IRequestHandler<CancelInviteRequestCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CancelInviteRequestCommandHandler> _logger = logger;
        public async Task<Result> Handle(CancelInviteRequestCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, token);
                if (board is null)
                {
                    _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                    return Result.Failure(BoardErrors.NotFound);
                }

                var inviteRequest = await _unitOfWork.GetRepository<BoardInviteRequest>().GetAsync(request.RequestId, token);
                if (inviteRequest is null)
                {
                    _logger.LogWarning("Board invite request with id '{requestId}' was not found.", request.RequestId);
                    return Result.Failure(BoardInviteErrors.NotFound);
                }

                inviteRequest.Status = (int)BoardInviteRequestStatuses.Canceled;

                _unitOfWork.GetRepository<BoardInviteRequest>().Update(inviteRequest);

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if(affectedRows == 0)
                {
                    _logger.LogError("Can't cancel board invite request to board with id '{boardId}'.", request.BoardId);
                    return Result.Failure(BoardInviteErrors.CantCancel(board.Name));
                }

                return Result.Success();
            }, cancellationToken);
        }
    }
}
