using Chat.Domain.ValueObjects;
using Common.Blocks.Entities;

namespace Chat.Domain.Entities
{
    public class BoardMessage : BaseEntity<MessageId>
    {
        public Guid BoardId { get; set; }
        public Guid MemberId { get; set; }
        public Guid AccountId { get; set; }
        public required string Message { get; set; }
        public required bool IsDeleted { get; set; } = false;

    }
}
