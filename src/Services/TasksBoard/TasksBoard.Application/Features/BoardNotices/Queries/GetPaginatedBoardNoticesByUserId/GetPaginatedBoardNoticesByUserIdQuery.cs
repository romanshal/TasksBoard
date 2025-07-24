using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserId
{
    public record GetPaginatedBoardNoticesByUserIdQuery : GetPaginatedListQuery<BoardNoticeDto>
    {
        public Guid UserId { get; set; }
    }
}
