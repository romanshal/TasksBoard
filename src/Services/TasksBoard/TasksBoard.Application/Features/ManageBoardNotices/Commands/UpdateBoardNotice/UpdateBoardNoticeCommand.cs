using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNotice
{
    public record UpdateBoardNoticeCommand : ICommand<Result<Guid>>
    {
        public required Guid BoardId { get; set; }
        public required Guid NoticeId { get; set; }
        public required Guid AccountId { get; set; }
        public required string Definition { get; set; }
        public required string BackgroundColor { get; set; }
        public required string Rotation { get; set; }
    }
}
