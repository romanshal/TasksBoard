using Common.Blocks.Models.DomainResults;
using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardInvites.Queries.GetBoardInviteRequestByToAccountId
{
    public record GetBoardInviteRequestByToAccountIdQuery : IRequest<Result<IEnumerable<BoardInviteRequestDto>>>
    {
        public required Guid AccountId { get; set; }
    }
}
