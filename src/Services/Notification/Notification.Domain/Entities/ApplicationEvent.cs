using Common.Blocks.Entities;
using Common.Blocks.ValueObjects;
using Notification.Domain.ValueObjects;

namespace Notification.Domain.Entities
{
    public class ApplicationEvent : BaseEntity<ApplicationEventId>
    {
        public required Guid EventId { get; set; }
        public required AccountId AccountId { get; set; }
        public required string EventType { get; set; }
        public required string Payload { get; set; }
        public bool Read { get; set; } = false;
    }
}
