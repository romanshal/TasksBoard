using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardNotice
{
    public record DeleteBoardNoticeCommand : ICommand<Result>
    {
        public required Guid BoardId { get; set; }
        public required Guid NoticeId { get; set; }
    }
}
