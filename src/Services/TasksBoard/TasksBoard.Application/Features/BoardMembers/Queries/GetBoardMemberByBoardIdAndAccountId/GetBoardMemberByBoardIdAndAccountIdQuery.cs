using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMemberByBoardIdAndAccountId
{
    public record GetBoardMemberByBoardIdAndAccountIdQuery : IRequest<BoardMemberDto>
    {
        public required Guid BoardId { get; set; }
        public required Guid AccountId { get; set; }
    }
}
