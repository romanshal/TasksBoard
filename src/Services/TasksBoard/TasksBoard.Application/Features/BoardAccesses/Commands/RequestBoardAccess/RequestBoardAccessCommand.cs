using Common.Blocks.Models.DomainResults;
using MediatR;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess
{
    public record RequestBoardAccessCommand : IRequest<Result<Guid>>
    {
        public required Guid BoardId { get; set; }
        public required Guid AccountId { get; set; }
    }
}
