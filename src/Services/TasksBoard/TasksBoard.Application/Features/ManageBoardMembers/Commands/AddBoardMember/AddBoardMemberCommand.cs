using MediatR;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember
{
    public record AddBoardMemberCommand : IRequest<Guid>
    {
        public required Guid BoardId { get; set; }
        public required Guid AccountId { get; set; }
        public required string Nickname { get; set; }
    }
}
