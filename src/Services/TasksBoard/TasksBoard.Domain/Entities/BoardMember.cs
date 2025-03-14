using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class BoardMember : BaseEntity
    {
        public Guid BoardId { get; set; }
        public Guid AccountId { get; set; }

        public virtual Board Boards { get; set; }
        public virtual ICollection<BoardMemberPermission> BoardMemberPermissions { get; set; }

        public void AddPermissions(IEnumerable<BoardMemberPermission> permissions)
        {
            this.BoardMemberPermissions = [.. permissions];
        }
    }
}
