using Common.Blocks.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Entities
{
    public class BoardAccessRequest : BaseEntity<BoardAccessId>
    {
        public required BoardId BoardId { get; set; }
        public required Guid AccountId { get; set; }
        public required int Status { get; set; }

        public virtual Board Board { get; set; } = default!;
    }
}
