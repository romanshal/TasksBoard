using MediatR;

namespace Notification.Application.Features.Notifications.Commands.SetNotificationsRead
{
    public class SetNotificationsReadCommand : IRequest<Unit>
    {
        public required Guid AccountId { get; set; }
        public required Guid[] NotificationIds { get; set; }
    }
}
