using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.Boards.Queries.GetBoards
{
    public class GetBoardsQuery : IRequest<IEnumerable<BoardDto>>
    {
    }
}
