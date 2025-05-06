using Chat.Application.DTOs;
using MediatR;

namespace Chat.Application.Features.BoardMessages.Queries.GetBoardMessagesByBoardId
{
    public class GetBoardMessagesByBoardIdQuery : IRequest<IEnumerable<BoardMessageDto>>
    {
        public required Guid BoardId { get; set; }
        public required int PageIndex { get; set; }
        public required int PageSize { get; set; }
    }
}
