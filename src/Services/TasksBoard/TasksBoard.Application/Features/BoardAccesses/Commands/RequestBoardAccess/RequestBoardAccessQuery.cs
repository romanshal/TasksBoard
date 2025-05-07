using MediatR;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess
{
    public class RequestBoardAccessQuery : IRequest<Guid>
    {
        public required Guid BoardId { get; set; }
        public required Guid AccountId { get; set; }
        public required string AccountName { get; set; }
        public required string AccountEmail { get; set; }
    }
}
