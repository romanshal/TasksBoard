using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess
{
    public record CancelBoardAccessCommand : ICommand<Result<Guid>>
    {
        public required Guid RequestId { get; set; }
    }
}
