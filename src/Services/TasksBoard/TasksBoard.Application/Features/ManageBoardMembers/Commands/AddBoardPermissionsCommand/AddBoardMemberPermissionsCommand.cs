using MediatR;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand
{
    public class AddBoardMemberPermissionsCommand : IRequest<Unit>
    {
        public Guid BoardId { get; set; }
        public Guid MemberId { get; set; }
        public Guid[] Permissions { get; set; }
    }
}
