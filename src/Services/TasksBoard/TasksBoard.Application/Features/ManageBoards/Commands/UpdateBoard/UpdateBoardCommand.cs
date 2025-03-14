using MediatR;

namespace TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard
{
    public class UpdateBoardCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
    }
}
