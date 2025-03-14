using MediatR;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardCommand
{
    public class DeleteBoardNoticeCommand : IRequest<Unit>
    {
        public required Guid BoardId { get; set; }
        public required Guid NoticeId { get; set; }
    }
}
