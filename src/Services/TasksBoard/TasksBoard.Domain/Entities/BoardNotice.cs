using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class BoardNotice : BaseEntity
    {
        public required Guid AuthorId { get; set; }
        public required string AuthorName { get; set; }
        public required Guid BoardId { get; set; }
        public required string Definition { get; set; }
        public required string BackgroundColor { get; set; }
        public required string Rotation { get; set; }
        public bool Completed { get; set; }

        public virtual Board Board { get; set; }
    }
}