using Common.Blocks.Models.DomainResults;
using MediatR;

namespace TasksBoard.Application.Features.ManageBoardAccesses.Commands.ResolveAccessRequest
{
    public class ResolveAccessRequestCommand : IRequest<Result<Guid>>
    {
        public required Guid BoardId { get; set; }
        public required Guid RequestId { get; set; }
        public required Guid ResolveUserId { get; set; }
        public required bool Decision { get; set; }
    }
}
