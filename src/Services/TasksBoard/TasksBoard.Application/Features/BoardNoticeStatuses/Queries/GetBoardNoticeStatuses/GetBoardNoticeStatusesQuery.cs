using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardNoticeStatuses.Queries.GetBoardNoticeStatuses
{
    public class GetBoardNoticeStatusesQuery : IRequest<IEnumerable<BoardNoticeStatusDto>>
    {
    }
}
