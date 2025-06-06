using MediatR;

namespace Chat.Application.Features.BoardMessages.Commands.DeleteBoardMessage
{
    public class DeleteBoardMessageCommand : IRequest<Unit>
    {
        public required Guid BoardId { get; set; }
        public required Guid BoardMessageId { get; set; }
    }
}
