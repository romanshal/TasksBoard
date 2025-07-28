using Common.Blocks.Models.DomainResults;
using MediatR;

namespace TasksBoard.Application.Features.BoardInvites.Commands.ResolveInviteRequest
{
    public record ResolveInviteRequestCommand : IRequest<Result<Guid>>
    {
        public required Guid BoardId { get; set; }
        public required Guid RequestId { get; set; }
        public required bool Decision { get; set; }
    }
}
