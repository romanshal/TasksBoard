using MediatR;

namespace TasksBoard.Application.Features.Boards.Commands.UpdateBoard
{
    public class UpdateBoardCommand : IRequest<Guid>
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
    }
}
