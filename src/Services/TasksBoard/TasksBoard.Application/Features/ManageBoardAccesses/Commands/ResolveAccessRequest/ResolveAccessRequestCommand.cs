using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.ManageBoardAccesses.Commands.ResolveAccessRequest
{
    public class ResolveAccessRequestCommand : ICommand<Result<Guid>>
    {
        public required Guid BoardId { get; set; }
        public required Guid RequestId { get; set; }
        public required Guid ResolveUserId { get; set; }
        public required bool Decision { get; set; }
    }
}
