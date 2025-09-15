using Chat.Application.DTOs;
using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace Chat.Application.Features.BoardMessages.Commands.UpdateBoardMessage
{
    public class UpdateBoardMessageCommand : ICommand<Result<BoardMessageDto>>
    {
        public required Guid BoardId { get; set; }
        public required Guid BoardMessageId { get; set; }
        public required string Message { get; set; }
    }
}
