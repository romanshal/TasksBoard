using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardPermission.Queries.GetBoardPermissions
{
    public class GetBoardPermissionsQuery : IRequest<IEnumerable<BoardPermissionDto>>
    {
    }
}
