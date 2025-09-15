using Chat.Application.DTOs;
using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace Chat.Application.Features.BoardMessages.Commands.CreateBoardMessage
{
    public class CreateBoardMessageCommand : ICommand<Result<BoardMessageDto>>
    {
        public required Guid BoardId { get; set; }
        public required Guid MemberId { get; set; }
        public required Guid AccountId { get; set; }
        public required string Message { get; set; }
    }
}
