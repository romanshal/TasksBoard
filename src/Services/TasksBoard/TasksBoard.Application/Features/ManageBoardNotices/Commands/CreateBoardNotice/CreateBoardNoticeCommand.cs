using MediatR;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice
{
    public class CreateBoardNoticeCommand : IRequest<Guid>
    {
        public required Guid AuthorId { get; set; }
        public required Guid BoardId { get; set; }
        public required Guid NoticeStatusId { get; set; }
        public required string Definition { get; set; }
    }
}
