using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using Notification.Application.Dtos;

namespace Notification.Application.Features.Notifications.Queries.GetNewNotificationsByAccountId
{
    public class GetNewNotificationsByAccountIdQuery : IQuery<Result<IEnumerable<NotificationDto>>>
    {
        public required Guid AccountId { get; set; }
    }
}
