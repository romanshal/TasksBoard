using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class BoardMember : BaseEntity
    {
        public Guid BoardId { get; set; }
        public Guid AccountId { get; set; }

        public virtual Board Board { get; set; }
        public virtual ICollection<BoardMemberPermission> BoardMemberPermissions { get; set; } = [];
    }
}
