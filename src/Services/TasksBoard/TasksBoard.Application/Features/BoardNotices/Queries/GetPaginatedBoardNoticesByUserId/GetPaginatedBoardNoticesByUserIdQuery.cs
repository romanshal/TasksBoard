using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNotices;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserId
{
    public class GetPaginatedBoardNoticesByUserIdQuery : GetPaginatedBoardNoticesQuery
    {
        public Guid UserId { get; set; }
    }
}
