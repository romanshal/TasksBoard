using Common.Blocks.Models.DomainResults;
using MediatR;

namespace TasksBoard.Application.Features.ManageBoardInvites.Command.CancelInviteRequest
{
    public class CancelInviteRequestCommand : IRequest<Result>
    {
        public required Guid BoardId { get; set; }
        public required Guid RequestId { get; set; }
    }
}
