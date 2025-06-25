namespace Common.Blocks.Entities
{
    public class OutboxEvent : BaseEntity
    {
        public required string EventType { get; set; }
        public required string Payload { get; set; }
        public required string Status { get; set; }
    }
}
