using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.BoardInvites.Commands.ResolveInviteRequest
{
    public record ResolveInviteRequestCommand : ICommand<Result<Guid>>
    {
        public required Guid BoardId { get; set; }
        public required Guid RequestId { get; set; }
        public required bool Decision { get; set; }
    }
}
