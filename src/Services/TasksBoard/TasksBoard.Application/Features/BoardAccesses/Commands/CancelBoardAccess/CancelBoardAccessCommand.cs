using MediatR;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess
{
    public class CancelBoardAccessCommand : IRequest<Guid>
    {
        public required Guid RequestId { get; set; }
    }
}
