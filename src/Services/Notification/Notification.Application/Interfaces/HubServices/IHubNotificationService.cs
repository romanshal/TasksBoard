using Notification.Application.Dtos;

namespace Notification.Application.Interfaces.HubServices
{
    public interface IHubNotificationService
    {
        Task NotifyUserAsync(string accountId, NotificationDto notification, CancellationToken cancellationToken = default);
    }
}
