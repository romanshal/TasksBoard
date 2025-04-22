using MediatR;

namespace TasksBoard.Application.Features.Boards.Commands.CreateBoard
{
    public class CreateBoardCommand : IRequest<Guid>
    {
        public required Guid OwnerId { get; set; }
        public required string Name { get; set; }
        public required string OwnerNickname { get; set; }
        public string Description { get; set; }
    }
}
