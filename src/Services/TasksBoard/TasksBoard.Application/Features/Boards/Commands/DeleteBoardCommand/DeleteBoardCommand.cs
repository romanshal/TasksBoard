using MediatR;

namespace TasksBoard.Application.Features.Boards.Commands.DeleteBoardCommand
{
    public class DeleteBoardCommand : IRequest<Unit>
    {
        public required Guid Id { get; set; }
    }
}
