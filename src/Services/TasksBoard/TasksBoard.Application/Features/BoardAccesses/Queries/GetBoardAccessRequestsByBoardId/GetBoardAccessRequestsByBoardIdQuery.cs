using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestsByBoardId
{
    public class GetBoardAccessRequestsByBoardIdQuery : IRequest<IEnumerable<BoardAccessRequestDto>>
    {
        public required Guid BoardId { get; set; }
    }
}
