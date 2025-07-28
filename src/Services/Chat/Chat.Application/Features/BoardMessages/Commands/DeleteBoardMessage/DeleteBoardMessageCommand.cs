using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Chat.Application.Features.BoardMessages.Commands.DeleteBoardMessage
{
    public class DeleteBoardMessageCommand : IRequest<Result>
    {
        public required Guid BoardId { get; set; }
        public required Guid BoardMessageId { get; set; }
    }
}
