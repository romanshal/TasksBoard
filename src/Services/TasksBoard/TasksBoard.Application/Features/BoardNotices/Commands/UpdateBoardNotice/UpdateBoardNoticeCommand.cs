using MediatR;

namespace TasksBoard.Application.Features.BoardNotices.Commands.UpdateBoardNotice
{
    public class UpdateBoardNoticeCommand : IRequest<Guid>
    {
        public required Guid Id { get; set; }
        public required string Definition { get; set; }
        public Guid NoticeStatusId { get; set; }
    }
}
