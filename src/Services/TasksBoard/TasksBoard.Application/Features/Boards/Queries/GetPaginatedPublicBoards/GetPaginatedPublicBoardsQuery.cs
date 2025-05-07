using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedPublicBoards
{
    public class GetPaginatedPublicBoardsQuery : GetPaginatedListQuery<BoardForViewDto>
    {
        public Guid AccountId { get; set; }
    }
}
