using Common.Blocks.Models;
using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNotices
{
    public class GetPaginatedBoardNoticesQuery : IRequest<PaginatedList<BoardNoticeDto>>
    {
        public required Guid BoardId { get; set; }
        public required int PageIndex { get; set; }
        public required int PageSize { get; set; }
    }
}
