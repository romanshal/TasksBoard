using MediatR;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess
{
    public record CancelBoardAccessCommand : IRequest<Guid>
    {
        public required Guid RequestId { get; set; }
    }
}
