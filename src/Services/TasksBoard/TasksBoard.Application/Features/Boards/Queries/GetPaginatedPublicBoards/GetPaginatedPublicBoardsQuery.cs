using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedPublicBoards
{
    public record GetPaginatedPublicBoardsQuery : GetPaginatedListQuery<BoardForViewDto>
    {
        public Guid AccountId { get; set; }
    }
}
