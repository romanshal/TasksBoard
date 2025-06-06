using Common.Blocks.Models;
using MediatR;
using Notification.Application.Dtos;

namespace Notification.Application.Features.Notifications.Queries.GetPaginatedNotificationsByAccountId
{
    public class GetAllPaginatedNotificationsByAccountIdQuery : IRequest<PaginatedList<NotificationDto>>
    {
        public required Guid AccountId { get; set; }
        public required int PageIndex { get; set; }
        public required int PageSize { get; set; }
    }
}
