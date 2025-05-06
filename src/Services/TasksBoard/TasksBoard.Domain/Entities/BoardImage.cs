using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class BoardImage : BaseEntity
    {
        public Guid BoardId { get; set; }
        public required string Extension { get; set; }
        public required byte[] Image { get; set; }

        public virtual Board Board { get; set; }
    }
}
