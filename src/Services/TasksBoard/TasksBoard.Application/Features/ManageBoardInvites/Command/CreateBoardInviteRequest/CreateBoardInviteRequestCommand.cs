using MediatR;

namespace TasksBoard.Application.Features.ManageBoardInvites.Command.CreateBoardInviteRequest
{
    public class CreateBoardInviteRequestCommand : IRequest<Guid>
    {
        public required Guid BoardId { get; set; }
        public required Guid FromAccountId { get; set; }
        public required string FromAccountName { get; set; }
        public required Guid ToAccountId { get; set; }
        public required string ToAccountName { get; set; }
        public required string ToAccountEmail { get; set; }
    }
}
