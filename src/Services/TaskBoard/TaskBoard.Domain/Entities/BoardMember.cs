using Common.Blocks.Entities;

namespace TaskBoard.Domain.Entities
{
    public class BoardMember : BaseEntity
    {
        public Guid AccountId { get; set; }

        public virtual ICollection<BoardMemberPermission> BoardMemberPermissions { get; set; }
    }
}
