using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNotices
{
    public class GetBoardNoticesQuery : IRequest<IEnumerable<BoardNoticeDto>>
    {
    }
}
