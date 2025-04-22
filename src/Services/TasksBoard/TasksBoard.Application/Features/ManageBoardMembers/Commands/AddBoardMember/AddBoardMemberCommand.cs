using MediatR;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember
{
    public class AddBoardMemberCommand : IRequest<Guid>
    {
        public Guid BoardId { get; set; } = Guid.Empty;
        public Guid UserId { get; set; }
        public required string Nickname { get; set; }
        public Guid[] Permissions { get; set; }
    }
}
