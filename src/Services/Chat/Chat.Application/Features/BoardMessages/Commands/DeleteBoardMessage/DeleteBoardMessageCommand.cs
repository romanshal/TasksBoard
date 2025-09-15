using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace Chat.Application.Features.BoardMessages.Commands.DeleteBoardMessage
{
    public class DeleteBoardMessageCommand : ICommand<Result>
    {
        public required Guid BoardId { get; set; }
        public required Guid BoardMessageId { get; set; }
    }
}
