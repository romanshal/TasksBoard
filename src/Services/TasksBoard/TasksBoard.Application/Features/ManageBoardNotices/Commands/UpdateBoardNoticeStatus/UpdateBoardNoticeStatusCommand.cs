using MediatR;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNoticeStatus
{
    public class UpdateBoardNoticeStatusCommand : IRequest<Guid>
    {
        public Guid BoardId { get; set; }
        public Guid NoticeId { get; set; }
        public bool Complete { get; set; }
    }
}
