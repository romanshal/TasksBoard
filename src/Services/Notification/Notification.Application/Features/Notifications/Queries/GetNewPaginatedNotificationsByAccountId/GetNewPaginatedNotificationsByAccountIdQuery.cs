using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Notification.Application.Dtos;

namespace Notification.Application.Features.Notifications.Queries.GetNewPaginatedNotificationsByAccountId
{
    public class GetNewPaginatedNotificationsByAccountIdQuery : IRequest<Result<PaginatedList<NotificationDto>>>
    {
        public required Guid AccountId { get; set; }
        public required int PageIndex { get; set; }
        public required int PageSize { get; set; }
    }
}
