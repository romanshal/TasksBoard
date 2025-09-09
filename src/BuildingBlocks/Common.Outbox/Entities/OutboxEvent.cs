using Common.Blocks.Entities;

namespace Common.Outbox.Entities
{
    public class OutboxEvent : BaseEntity
    {
        public required string EventType { get; set; }
        public required string Payload { get; set; }
        public required string Status { get; set; }
    }
}
