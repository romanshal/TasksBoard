using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember
{
    public record DeleteBoardMemberCommand : ICommand<Result>
    {
        public Guid BoardId { get; set; }
        public Guid MemberId { get; set; }
        public Guid RemoveByUserId { get; set; }
    }
}
