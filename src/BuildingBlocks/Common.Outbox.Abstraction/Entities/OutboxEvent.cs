using Common.Blocks.Entities;
using Common.Outbox.Abstraction.ValueObjects;

namespace Common.Outbox.Abstraction.Entities
{
    public class OutboxEvent : BaseEntity<OutboxId>
    {
        public required string EventType { get; set; }
        public required string Payload { get; set; }
        public required string Status { get; set; }
    }
}
