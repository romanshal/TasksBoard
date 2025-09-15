using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardInvites.Queries.GetBoardInviteRequestByToAccountId
{
    public record GetBoardInviteRequestByToAccountIdQuery : IQuery<Result<IEnumerable<BoardInviteRequestDto>>>
    {
        public required Guid AccountId { get; set; }
    }
}
