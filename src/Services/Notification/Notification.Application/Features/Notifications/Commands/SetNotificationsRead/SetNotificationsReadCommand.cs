using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace Notification.Application.Features.Notifications.Commands.SetNotificationsRead
{
    public class SetNotificationsReadCommand : ICommand<Result>
    {
        public required Guid AccountId { get; set; }
        public required Guid[] NotificationIds { get; set; }
    }
}
