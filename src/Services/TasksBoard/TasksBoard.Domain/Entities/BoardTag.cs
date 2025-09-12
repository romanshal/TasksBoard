using Common.Blocks.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Entities
{
    public class BoardTag : BaseEntity<BoardTagId>
    {
        public BoardId BoardId { get; set; } = default!;
        public required string Tag { get; set; }

        public virtual Board Board { get; set; } = default!;
    }
}
