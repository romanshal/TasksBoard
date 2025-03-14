using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class BoardPermission : BaseEntity
    {
        public required string Name { get; set; }
        public required int AccessLevel { get; set; }

        public virtual ICollection<BoardMemberPermission> BoardMemberPermissions { get; set; }
    }
}
