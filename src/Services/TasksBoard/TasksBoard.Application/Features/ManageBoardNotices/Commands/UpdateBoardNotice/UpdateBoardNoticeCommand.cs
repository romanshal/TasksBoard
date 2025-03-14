using MediatR;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNotice
{
    public class UpdateBoardNoticeCommand : IRequest<Guid>
    {
        public required Guid BoardId { get; set; }
        public required Guid NoticeId { get; set; }
        public required string Definition { get; set; }
        public Guid NoticeStatusId { get; set; }
    }
}
