using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.ManageBoardInvites.Queries.GetBoardInviteRequestsByBoardId
{
    public class GetBoardInviteRequestsByBoardIdQuery : IRequest<IEnumerable<BoardInviteRequestDto>>
    {
        public required Guid BoardId { get; set; }
    }
}
