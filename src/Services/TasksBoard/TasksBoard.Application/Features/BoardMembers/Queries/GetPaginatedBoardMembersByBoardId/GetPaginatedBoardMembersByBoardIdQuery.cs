using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetPaginatedBoardMembersByBoardId
{
    public record GetPaginatedBoardMembersByBoardIdQuery : GetPaginatedListQuery<BoardMemberDto>
    {
        public required Guid BoardId { get; set; }
    }
}
