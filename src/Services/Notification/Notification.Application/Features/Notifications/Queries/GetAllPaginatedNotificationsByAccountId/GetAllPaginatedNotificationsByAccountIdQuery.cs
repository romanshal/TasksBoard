using Common.Blocks.CQRS;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using Notification.Application.Dtos;

namespace Notification.Application.Features.Notifications.Queries.GetPaginatedNotificationsByAccountId
{
    public class GetAllPaginatedNotificationsByAccountIdQuery : IQuery<Result<PaginatedList<NotificationDto>>>
    {
        public required Guid AccountId { get; set; }
        public required int PageIndex { get; set; }
        public required int PageSize { get; set; }
    }
}
