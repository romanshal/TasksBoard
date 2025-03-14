using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoardsByUserId
{
    public class GetPaginatedBoardsByUserIdQuery : GetPaginatedListQuery<BoardDto>
    {
        public required Guid UserId { get; set; }
    }
}
