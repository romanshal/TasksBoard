using MediatR;

namespace TasksBoard.Application.Features.BoardNotices.Commands.DeleteBoardCommand
{
    public class DeleteBoardNoticeCommand : IRequest<Unit>
    {
        public required Guid Id { get; set; }
    }
}
