using MediatR;

namespace TasksBoard.Application.Features.BoardInvites.Commands.ResolveInviteRequest
{
    public record ResolveInviteRequestCommand : IRequest<Guid>
    {
        public required Guid BoardId { get; set; }
        public required Guid RequestId { get; set; }
        public required bool Decision { get; set; }
    }
}
