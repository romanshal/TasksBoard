using Common.Blocks.Models.DomainResults;
using MediatR;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember
{
    public record DeleteBoardMemberCommand : IRequest<Result>
    {
        public Guid BoardId { get; set; }
        public Guid MemberId { get; set; }
        public Guid RemoveByUserId { get; set; }
    }
}
