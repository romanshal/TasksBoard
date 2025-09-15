using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember
{
    public record AddBoardMemberCommand : ICommand<Result<Guid>>
    {
        public required Guid BoardId { get; set; }
        public required Guid AccountId { get; set; }
    }
}
