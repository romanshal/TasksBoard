using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Notification.Application.Features.Notifications.Commands.SetNotificationsRead
{
    public class SetNotificationsReadCommand : IRequest<Result>
    {
        public required Guid AccountId { get; set; }
        public required Guid[] NotificationIds { get; set; }
    }
}
