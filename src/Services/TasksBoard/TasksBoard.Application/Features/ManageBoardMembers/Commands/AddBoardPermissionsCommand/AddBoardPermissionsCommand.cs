using MediatR;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand
{
    public class AddBoardPermissionsCommand : IRequest<Unit>
    {
        public Guid BoardId { get; set; }
        public Guid UserId { get; set; }
        public Guid[] Permissions { get; set; }
    }
}
