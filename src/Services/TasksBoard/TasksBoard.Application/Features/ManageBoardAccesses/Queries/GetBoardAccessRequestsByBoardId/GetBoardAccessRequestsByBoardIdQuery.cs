using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.ManageBoardAccesses.Queries.GetBoardAccessRequestsByBoardId
{
    public class GetBoardAccessRequestsByBoardIdQuery : IQuery<Result<IEnumerable<BoardAccessRequestDto>>>
    {
        public required Guid BoardId { get; set; }
    }
}
