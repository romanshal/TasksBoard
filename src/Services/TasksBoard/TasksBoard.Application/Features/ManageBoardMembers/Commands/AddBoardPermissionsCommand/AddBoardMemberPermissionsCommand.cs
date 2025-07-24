using MediatR;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand
{
    public record AddBoardMemberPermissionsCommand : IRequest<Unit>
    {
        public required Guid BoardId { get; set; }
        public required Guid MemberId { get; set; }
        public required Guid AccountId { get; set; }
        public required Guid[] Permissions { get; set; }
    }
}
