using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMembersByBoardId
{
    public record GetBoardMembersByBoardIdQuery : IQuery<Result<IEnumerable<BoardMemberDto>>>
    {
        public required Guid BoardId { get; set; }
    }
}
