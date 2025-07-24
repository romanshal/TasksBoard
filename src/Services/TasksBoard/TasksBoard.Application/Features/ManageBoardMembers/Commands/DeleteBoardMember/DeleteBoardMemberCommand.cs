using MediatR;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember
{
    public record DeleteBoardMemberCommand : IRequest<Unit>
    {
        public Guid BoardId { get; set; }
        public Guid MemberId { get; set; }
        public Guid RemoveByUserId { get; set; }
    }
}
