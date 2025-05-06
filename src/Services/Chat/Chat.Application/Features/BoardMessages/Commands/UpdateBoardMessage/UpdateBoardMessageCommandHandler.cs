using AutoMapper;
using Chat.Domain.Interfaces.UnitOfWorks;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Features.BoardMessages.Commands.UpdateBoardMessage
{
    public class UpdateBoardMessageCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateBoardMessageCommandHandler> logger,
        IMapper mapper) : IRequestHandler<UpdateBoardMessageCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<UpdateBoardMessageCommandHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        public async Task<Guid> Handle(UpdateBoardMessageCommand request, CancellationToken cancellationToken)
        {
            //TODO: check board exist

            var boardMessage = await _unitOfWork.GetBoardMessagesRepository().GetAsync(request.BoardMessageId);
            if (boardMessage is null)
            {
                _logger.LogWarning($"Board message with id '{request.BoardMessageId}' not found.");
                throw new NotFoundException($"Board message with id '{request.BoardMessageId}' not found.");
            }

            boardMessage.Message = request.Message;

            await _unitOfWork.GetBoardMessagesRepository().Update(boardMessage, true, cancellationToken);

            if (boardMessage.Id == Guid.Empty)
            {
                _logger.LogError("Can't update board message.");
                throw new ArgumentException(nameof(boardMessage));
            }

            _logger.LogInformation($"Board message with id '{boardMessage.Id}' updated in board with id '{request.BoardId}'.");

            return boardMessage.Id;
        }
    }
}
