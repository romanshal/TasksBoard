using Common.Blocks.Models.DomainResults;
using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.ManageBoardMembers.Queries.GetBoardMemberPermissions
{
    public class GetBoardMemberPermissionsQuery : IRequest<Result<IEnumerable<BoardMemberPermissionDto>>>
    {
        public required Guid BoardId { get; set; }
        public required Guid MemberId { get; set; }
    }
}
