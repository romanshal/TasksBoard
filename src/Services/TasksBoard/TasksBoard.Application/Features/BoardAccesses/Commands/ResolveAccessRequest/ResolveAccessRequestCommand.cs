using MediatR;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.ResolveAccessRequest
{
    public class ResolveAccessRequestCommand : IRequest<Guid>
    {
        public required Guid BoardId { get; set; }
        public required Guid RequestId { get; set; }
        public required bool Decision { get; set; }
    }
}
