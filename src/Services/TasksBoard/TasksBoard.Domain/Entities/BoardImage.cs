using Common.Blocks.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Entities
{
    public class BoardImage : BaseEntity<BoardImageId>
    {
        public BoardId BoardId { get; set; } = default!;
        public required string Extension { get; set; }
        public required byte[] Image { get; set; }

        public virtual Board Board { get; set; } = default!;
    }
}
