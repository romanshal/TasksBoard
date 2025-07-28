using Common.Blocks.Models.DomainResults;
using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.ManageBoardAccesses.Queries.GetBoardAccessRequestsByBoardId
{
    public class GetBoardAccessRequestsByBoardIdQuery : IRequest<Result<IEnumerable<BoardAccessRequestDto>>>
    {
        public required Guid BoardId { get; set; }
    }
}
