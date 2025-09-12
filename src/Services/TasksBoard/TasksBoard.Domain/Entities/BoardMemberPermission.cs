using Common.Blocks.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Entities
{
    public class BoardMemberPermission : BaseEntity<MemberPermissionId>
    {
        public BoardMemberId BoardMemberId { get; set; } = default!;
        public required BoardPermissionId BoardPermissionId { get; set; }

        public virtual BoardMember BoardMember { get; set; } = default!;
        public virtual BoardPermission BoardPermission { get; set; } = default!;
    }
}
