using MediatR;

namespace TasksBoard.Application.Features.Boards.Commands.CreateBoard
{
    public record CreateBoardCommand : IRequest<Guid>
    {
        public required Guid OwnerId { get; set; }
        public required string Name { get; set; }
        public required string OwnerNickname { get; set; }
        public string? Description { get; set; }
        public string[]? Tags { get; set; }
        public bool Public { get; set; }
        public byte[]? Image { get; set; }
        public string? ImageExtension { get; set; }
    }
}
