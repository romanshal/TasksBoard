using Notification.Domain.Constants;

namespace Notification.Application.Dtos
{
    public class NotificationDto
    {
        public required Guid Id { get; set; }
        public required string Type { get; set; }
        public required Dictionary<NotificationLinkTypes, string> Payload { get; set; }
        public bool Read { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
