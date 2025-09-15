using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardPermission.Queries.GetBoardPermissions
{
    public class GetBoardPermissionsQuery : IQuery<Result<IEnumerable<BoardPermissionDto>>>
    {
    }
}
