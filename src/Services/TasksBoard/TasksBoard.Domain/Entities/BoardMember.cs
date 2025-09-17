using Common.Blocks.Entities;
using Common.Blocks.ValueObjects;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Entities
{
    public class BoardMember : BaseEntity<BoardMemberId>
    {
        public required BoardId BoardId { get; set; }
        public required AccountId AccountId { get; set; }

        public virtual Board Board { get; set; } = default!;
        public virtual ICollection<BoardMemberPermission> BoardMemberPermissions { get; set; } = [];
    }
}
