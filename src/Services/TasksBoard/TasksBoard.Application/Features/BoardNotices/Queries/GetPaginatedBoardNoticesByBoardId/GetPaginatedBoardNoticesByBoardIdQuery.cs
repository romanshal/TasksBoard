using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByBoardId
{
    public record GetPaginatedBoardNoticesByBoardIdQuery : GetPaginatedListQuery<BoardNoticeDto>
    {
        public required Guid BoardId { get; set; }
    }
}
