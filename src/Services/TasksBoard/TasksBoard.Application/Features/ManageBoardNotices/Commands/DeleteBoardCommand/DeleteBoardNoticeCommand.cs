using Common.Blocks.Models.DomainResults;
using MediatR;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardCommand
{
    public record DeleteBoardNoticeCommand : IRequest<Result>
    {
        public required Guid BoardId { get; set; }
        public required Guid NoticeId { get; set; }
    }
}
