using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.ManageBoardMembers.Queries.GetBoardMemberPermissions
{
    public class GetBoardMemberPermissionsQuery : IQuery<Result<IEnumerable<BoardMemberPermissionDto>>>
    {
        public required Guid BoardId { get; set; }
        public required Guid MemberId { get; set; }
    }
}
