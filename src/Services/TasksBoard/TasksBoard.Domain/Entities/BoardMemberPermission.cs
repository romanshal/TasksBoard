using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class BoardMemberPermission : BaseEntity
    {
        public required Guid BoardMemberId { get; set; }
        public required Guid BoardPermissionId { get; set; }

        public virtual BoardMember BoardMember { get; set; }
        public virtual BoardPermission BoardPermission { get; set; }
    }
}
