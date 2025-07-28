using Common.Blocks.Models.DomainResults;
using MediatR;
using Notification.Application.Dtos;

namespace Notification.Application.Features.Notifications.Queries.GetNewNotificationsByAccountId
{
    public class GetNewNotificationsByAccountIdQuery : IRequest<Result<IEnumerable<NotificationDto>>>
    {
        public required Guid AccountId { get; set; }
    }
}
