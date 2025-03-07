using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.Boards.Queries.GetBoardById
{
    public class GetBoardByIdQuery : IRequest<BoardDto>
    {
        public Guid Id { get; set; }
    }
}
