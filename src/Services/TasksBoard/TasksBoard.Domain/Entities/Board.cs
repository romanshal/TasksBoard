using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class Board : BaseEntity
    {
        public required Guid OwnerId { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<BoardNotice> Notices { get; set; }
    }
}
