using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class BoardTag : BaseEntity
    {
        public Guid BoardId { get; set; }
        public required string Tag { get; set; }

        public virtual Board Board { get; set; }
    }
}
