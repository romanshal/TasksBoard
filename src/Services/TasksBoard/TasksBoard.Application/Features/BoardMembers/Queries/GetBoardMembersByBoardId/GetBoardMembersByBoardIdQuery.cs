using Common.Blocks.Models.DomainResults;
using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMembersByBoardId
{
    public record GetBoardMembersByBoardIdQuery : IRequest<Result<IEnumerable<BoardMemberDto>>>
    {
        public required Guid BoardId { get; set; }
    }
}
