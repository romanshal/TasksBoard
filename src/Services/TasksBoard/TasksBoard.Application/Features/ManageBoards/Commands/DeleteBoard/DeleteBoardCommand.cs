using MediatR;

namespace TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard
{
    public record DeleteBoardCommand : IRequest<Unit>
    {
        public required Guid Id { get; set; }
    }
}
