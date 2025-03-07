using Common.Blocks.Entities;

namespace TaskBoard.Infrastructure.Entities
{
    public class BoardNotice : BaseEntity
    {
        public required Guid AuthorId { get; set; }
        public required Guid BoardId { get; set; }
        public required string Definition { get; set; }
        public bool Status { get; set; }

        public virtual Board Board { get; set; }
    }
}