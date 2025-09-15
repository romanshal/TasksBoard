using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMemberByBoardIdAndAccountId
{
    public record GetBoardMemberByBoardIdAndAccountIdQuery : IQuery<Result<BoardMemberDto>>
    {
        public required Guid BoardId { get; set; }
        public required Guid AccountId { get; set; }
    }
}
