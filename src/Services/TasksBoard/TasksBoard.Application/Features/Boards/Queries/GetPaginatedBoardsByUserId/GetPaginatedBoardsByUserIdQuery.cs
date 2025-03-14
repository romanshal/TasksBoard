using TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoards;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoardsByUserId
{
    public class GetPaginatedBoardsByUserIdQuery : GetPaginatedBoardsQuery
    {
        public required Guid UserId { get; set; }
    }
}
