using Common.Blocks.Models;
using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoards
{
    public class GetPaginatedBoardsQuery : IRequest<PaginatedList<BoardDto>>
    {
        public required int PageIndex { get; set; }
        public required int PageSize { get; set; }
    }
}
