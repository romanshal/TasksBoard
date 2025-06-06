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
                _logger.LogWarning($"Board message with id '{request.BoardMessageId}' not found.");
                throw new NotFoundException($"Board message with id '{request.BoardMessageId}' not found.");
            }

            boardMessage.IsDeleted = true;

            await _unitOfWork.GetBoardMessagesRepository().Update(boardMessage, true, cancellationToken);

            _logger.LogInformation($"Board message with id '{boardMessage.Id}' logical deleted in board with id '{request.BoardId}'.");

            return Unit.Value;
        }
    }
}
