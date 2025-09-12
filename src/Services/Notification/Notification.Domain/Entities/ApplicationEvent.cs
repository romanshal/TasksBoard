using Common.Blocks.Entities;
using Notification.Domain.ValueObjects;

namespace Notification.Domain.Entities
{
    public class ApplicationEvent : BaseEntity<ApplicationEventId>
    {
        public required Guid EventId { get; set; }
        public required Guid AccountId { get; set; }
        public required string EventType { get; set; }
        public required string Payload { get; set; }
        public bool Read { get; set; } = false;
    }
}
