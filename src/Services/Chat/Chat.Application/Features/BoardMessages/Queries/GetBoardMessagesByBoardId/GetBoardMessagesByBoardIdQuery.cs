using Chat.Application.DTOs;
using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace Chat.Application.Features.BoardMessages.Queries.GetBoardMessagesByBoardId
{
    public class GetBoardMessagesByBoardIdQuery : IQuery<Result<IEnumerable<BoardMessageDto>>>
    {
        public required Guid BoardId { get; set; }
        public required int PageIndex { get; set; }
        public required int PageSize { get; set; }
    }
}
