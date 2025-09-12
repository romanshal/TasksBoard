using Common.Blocks.Entities;
using Common.Outbox.ValueObjects;

namespace Common.Outbox.Entities
{
    public class OutboxEvent : BaseEntity<OutboxId>
    {
        public required string EventType { get; set; }
        public required string Payload { get; set; }
        public required string Status { get; set; }
    }
}
