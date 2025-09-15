using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess
{
    public record RequestBoardAccessCommand : ICommand<Result<Guid>>
    {
        public required Guid BoardId { get; set; }
        public required Guid AccountId { get; set; }
    }
}
