using Common.Blocks.Models.DomainResults;
using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestByAccountId
{
    public record GetBoardAccessRequestByAccountIdQuery : IRequest<Result<IEnumerable<BoardAccessRequestDto>>>
    {
        public required Guid AccountId { get; set; }
    }
}
