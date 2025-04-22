using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMembersByBoardId
{
    public class GetBoardMembersByBoardIdQuery : IRequest<IEnumerable<BoardMemberDto>>
    {
        public required Guid BoardId { get; set; }
    }
}
