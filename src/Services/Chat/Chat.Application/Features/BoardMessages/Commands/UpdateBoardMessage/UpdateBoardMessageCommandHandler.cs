using AutoMapper;
using Chat.Application.DTOs;
using Chat.Domain.Interfaces.UnitOfWorks;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Features.BoardMessages.Commands.UpdateBoardMessage
{
    public class UpdateBoardMessageCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateBoardMessageCommandHandler> logger,
        IMapper mapper) : IRequestHandler<UpdateBoardMessageCommand, BoardMessageDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<UpdateBoardMessageCommandHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        public async Task<BoardMessageDto> Handle(UpdateBoardMessageCommand request, CancellationToken cancellationToken)
        {
            //TODO: check board exist

            var boardMessage = await _unitOfWork.GetBoardMessagesRepository().GetAsync(request.BoardMessageId);
            if (boardMessage is null)
            {
                _logger.LogWarning("Board message with id '{boardMessageId}' not found.", request.BoardMessageId);
                throw new NotFoundException($"Board message with id '{request.BoardMessageId}' not found.");
            }

            boardMessage.Message = request.Message;

            _unitOfWork.GetBoardMessagesRepository().Update(boardMessage);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0 || boardMessage.Id == Guid.Empty)
            {
                _logger.LogError("Can't update board message.");
                throw new ArgumentException(nameof(boardMessage));
            }

            _logger.LogInformation("Board message with id '{id}' updated in board with id '{boardId}'.", boardMessage.Id, request.BoardId);

            var boardMessageDto = _mapper.Map<BoardMessageDto>(boardMessage);

            return boardMessageDto;
        }
    }
}
