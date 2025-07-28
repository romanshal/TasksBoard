using Chat.Domain.Constants.Errors.DomainErrors;
using Chat.Domain.Interfaces.UnitOfWorks;
using Common.Blocks.Exceptions;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Features.BoardMessages.Commands.DeleteBoardMessage
{
    public class DeleteBoardMessageCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteBoardMessageCommandHandler> logger) : IRequestHandler<DeleteBoardMessageCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<DeleteBoardMessageCommandHandler> _logger = logger;

        public async Task<Result> Handle(DeleteBoardMessageCommand request, CancellationToken cancellationToken)
        {
            //TODO: check board exist

            var boardMessage = await _unitOfWork.GetBoardMessagesRepository().GetAsync(request.BoardMessageId, cancellationToken);
            if (boardMessage is null)
            {
                _logger.LogWarning("Board message with id '{boardMessageId}' was not found.", request.BoardMessageId);
                return Result.Failure(BoardMessageErrors.NotFound);

                //throw new NotFoundException($"Board message with id '{request.BoardMessageId}' not found.");
            }

            boardMessage.IsDeleted = true;

            _unitOfWork.GetBoardMessagesRepository().Update(boardMessage);
            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0)
            {
                _logger.LogError("Can't delete board message with id '{messageId}' from board with id '{boardId}'.", request.BoardMessageId, request.BoardId);
                return Result.Failure(BoardMessageErrors.CantDelete);

                //throw new ArgumentException("Can't save new board message.");
            }

            _logger.LogInformation("Board message with id '{messageId}' was logical deleted in board with id '{boardId}'.", boardMessage.Id, request.BoardId);

            return Result.Success();
        }
    }
}
