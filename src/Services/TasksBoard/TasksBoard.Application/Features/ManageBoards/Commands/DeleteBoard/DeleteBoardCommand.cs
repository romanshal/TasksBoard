using MediatR;

namespace TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard
{
    public class DeleteBoardCommand : IRequest<Unit>
    {
        public required Guid Id { get; set; }
    }
}
