using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMemberPermissions
{
    public record AddBoardMemberPermissionsCommand : ICommand<Result>
    {
        public required Guid BoardId { get; set; }
        public required Guid MemberId { get; set; }
        public required Guid AccountId { get; set; }
        public required Guid[] Permissions { get; set; }
    }
}
