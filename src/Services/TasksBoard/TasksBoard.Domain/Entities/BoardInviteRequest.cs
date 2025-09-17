using Common.Blocks.Entities;
using Common.Blocks.ValueObjects;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Entities
{
    public class BoardInviteRequest : BaseEntity<BoardInviteId>
    {
        public required BoardId BoardId { get; set; }
        public required AccountId FromAccountId { get; set; }
        public required AccountId ToAccountId { get; set; }
        public required int Status { get; set; }

        public virtual Board Board { get; set; } = default!;
    }
}
