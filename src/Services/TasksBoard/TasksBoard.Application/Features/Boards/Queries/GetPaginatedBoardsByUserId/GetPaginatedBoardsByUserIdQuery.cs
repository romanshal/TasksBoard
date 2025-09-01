using TasksBoard.Application.DTOs.Boards;
using TasksBoard.Application.Models;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoardsByUserId
{
    public record GetPaginatedBoardsByUserIdQuery : GetPaginatedListQuery<BoardForViewDto>
    {
        public required Guid UserId { get; set; }
        public string? Query { get; set; }
    }
}
