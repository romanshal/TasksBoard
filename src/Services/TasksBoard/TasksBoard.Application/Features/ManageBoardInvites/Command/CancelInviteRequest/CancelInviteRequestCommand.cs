using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.ManageBoardInvites.Command.CancelInviteRequest
{
    public class CancelInviteRequestCommand : ICommand<Result>
    {
        public required Guid BoardId { get; set; }
        public required Guid RequestId { get; set; }
    }
}
