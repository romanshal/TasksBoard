using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardInvites.Queries.GetBoardInviteRequestByToAccountId
{
    public class GetBoardInviteRequestByToAccountIdQuery : IRequest<IEnumerable<BoardInviteRequestDto>>
    {
        public required Guid AccountId { get; set; }
    }
}
