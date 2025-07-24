using AutoMapper;
using Chat.Application.DTOs;
using Chat.Domain.Entities;
using Chat.Domain.Interfaces.UnitOfWorks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Features.BoardMessages.Commands.CreateBoardMessage
{
    public class CreateBoardMessageCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateBoardMessageCommandHandler> logger,
        IMapper mapper) : IRequestHandler<CreateBoardMessageCommand, BoardMessageDto>
    {

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CreateBoardMessageCommandHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<BoardMessageDto> Handle(CreateBoardMessageCommand request, CancellationToken cancellationToken)
        {
            //TODO: check board exist

            var boardMessage = _mapper.Map<BoardMessage>(request);

            _unitOfWork.GetBoardMessagesRepository().Add(boardMessage);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if ( affectedRows == 0 || boardMessage.Id == Guid.Empty)
            {
                _logger.LogError("Can't create new board message to board with id '{boardId}'.", request.BoardId);
                throw new ArgumentException(nameof(boardMessage));
            }

            var createdBoardMessageDto = _mapper.Map<BoardMessageDto>(boardMessage);

            _logger.LogInformation("Board message with id '{id}' added to board with id '{boardId}'.", boardMessage.Id, request.BoardId);

            return createdBoardMessageDto;
        }
    }
}
