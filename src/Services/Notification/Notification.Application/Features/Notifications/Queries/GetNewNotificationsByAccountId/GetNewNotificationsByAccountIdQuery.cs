using MediatR;
using Notification.Application.Dtos;

namespace Notification.Application.Features.Notifications.Queries.GetNewNotificationsByAccountId
{
    public class GetNewNotificationsByAccountIdQuery : IRequest<IEnumerable<NotificationDto>>
    {
        public required Guid AccountId { get; set; }
    }
}
