using MediatR;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardCommand
{
    public class DeleteBoardNoticeCommand : IRequest<Unit>
    {
        public required Guid Id { get; set; }
    }
}
