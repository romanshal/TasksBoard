using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestByAccountId
{
    public record GetBoardAccessRequestByAccountIdQuery : IQuery<Result<IEnumerable<BoardAccessRequestDto>>>
    {
        public required Guid AccountId { get; set; }
    }
}
