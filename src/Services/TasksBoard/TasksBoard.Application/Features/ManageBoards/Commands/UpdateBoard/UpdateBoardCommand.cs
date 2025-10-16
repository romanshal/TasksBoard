using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard
{
    public record UpdateBoardCommand : ICommand<Result<Guid>>
    {
        public required Guid BoardId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string[] Tags { get; set; } = [];
        public bool Public { get; set; }
        public byte[]? Image { get; set; }
        public string? ImageExtension { get; set; }
    }
}
