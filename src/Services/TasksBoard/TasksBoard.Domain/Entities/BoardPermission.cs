using Common.Blocks.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Entities
{
    public class BoardPermission : BaseEntity<BoardPermissionId>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int AccessLevel { get; set; }

        public virtual ICollection<BoardMemberPermission> BoardMemberPermissions { get; set; } = [];
    }
}
