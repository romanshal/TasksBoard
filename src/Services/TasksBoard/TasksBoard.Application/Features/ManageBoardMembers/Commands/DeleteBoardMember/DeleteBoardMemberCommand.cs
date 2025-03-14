using MediatR;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember
{
    public class DeleteBoardMemberCommand : IRequest<Unit>
    {
        public Guid BoardId { get; set; }
        public Guid UserId { get; set; }
    }
}
