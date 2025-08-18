using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class BoardAccessRequest : BaseEntity
    {
        public required Guid BoardId { get; set; }
        public required Guid AccountId { get; set; }
        public required int Status { get; set; }

        public virtual Board Board { get; set; }
    }
}
