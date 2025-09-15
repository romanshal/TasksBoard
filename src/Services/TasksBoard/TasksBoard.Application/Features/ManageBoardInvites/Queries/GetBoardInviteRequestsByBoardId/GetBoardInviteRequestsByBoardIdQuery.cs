using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.ManageBoardInvites.Queries.GetBoardInviteRequestsByBoardId
{
    public class GetBoardInviteRequestsByBoardIdQuery : IQuery<Result<IEnumerable<BoardInviteRequestDto>>>
    {
        public required Guid BoardId { get; set; }
    }
}
