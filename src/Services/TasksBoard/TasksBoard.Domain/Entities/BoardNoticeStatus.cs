using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class BoardNoticeStatus : BaseEntity
    {
        public required string Name { get; set; }

        public virtual ICollection<BoardNotice> BoardNotices { get; set; }
    }
}
