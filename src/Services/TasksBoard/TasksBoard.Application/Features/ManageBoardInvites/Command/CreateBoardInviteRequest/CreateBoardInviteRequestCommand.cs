using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.ManageBoardInvites.Command.CreateBoardInviteRequest
{
    public class CreateBoardInviteRequestCommand : ICommand<Result<Guid>>
    {
        public required Guid BoardId { get; set; }
        public required Guid FromAccountId { get; set; }
        public required Guid ToAccountId { get; set; }
    }
}
