using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class BoardNotice : BaseEntity
    {
        public required Guid AuthorId { get; set; }
        public required Guid BoardId { get; set; }
        public required Guid NoticeStatusId { get; set; }
        public required string Definition { get; set; }

        public virtual Board Board { get; set; }
        public virtual BoardNoticeStatus BoardNoticeStatus { get; set; }
    }
}