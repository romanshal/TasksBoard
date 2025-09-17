using Common.Blocks.Entities;
using Common.Blocks.ValueObjects;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Entities
{
    public class BoardNotice : BaseEntity<BoardNoticeId>
    {
        public required AccountId AuthorId { get; set; }
        public required BoardId BoardId { get; set; }
        public required string Definition { get; set; }
        public required string BackgroundColor { get; set; }
        public required string Rotation { get; set; }
        public bool Completed { get; set; }

        public virtual Board Board { get; set; } = default!;
    }
}