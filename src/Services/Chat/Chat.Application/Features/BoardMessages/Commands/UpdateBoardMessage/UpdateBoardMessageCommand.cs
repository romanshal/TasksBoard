using Chat.Application.DTOs;
using MediatR;

namespace Chat.Application.Features.BoardMessages.Commands.UpdateBoardMessage
{
    public class UpdateBoardMessageCommand : IRequest<BoardMessageDto>
    {
        public required Guid BoardId { get; set; }
        public required Guid BoardMessageId { get; set; }
        public required string Message { get; set; }
    }
}
