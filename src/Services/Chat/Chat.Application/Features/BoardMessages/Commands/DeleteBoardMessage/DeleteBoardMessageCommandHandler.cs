using Chat.Domain.Interfaces.UnitOfWorks;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Features.BoardMessages.Commands.DeleteBoardMessage
{
    public class DeleteBoardMessageCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteBoardMessageCommandHandler> logger) : IRequestHandler<DeleteBoardMessageCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<DeleteBoardMessageCommandHandler> _logger = logger;

        public async Task<Unit> Handle(DeleteBoardMessageCommand request, CancellationToken cancellationToken)
        {
            //TODO: check board exist

            var boardMessage = await _unitOfWork.GetBoardMessagesRepository().GetAsync(request.BoardMessageId, cancellationToken);
            if (boardMessage is null)
            {
                _logger.LogWarning("Board message with id '{boardMessageId}' not found.", request.BoardMessageId);
                throw new NotFoundException($"Board message with id '{request.BoardMessageId}' not found.");
            }

            boardMessage.IsDeleted = true;

            _unitOfWork.GetBoardMessagesRepository().Update(boardMessage);
            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if(affectedRows == 0)
            {
                _logger.LogError("Can't save new board message.");
                throw new ArgumentException("Can't save new board message.");
            }

            _logger.LogInformation("Board message with id '{id}' logical deleted in board with id '{boardId}'.", boardMessage.Id, request.BoardId);

            return Unit.Value;
        }
    }
}
