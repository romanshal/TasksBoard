using Notification.Application.Dtos;

namespace Notification.API.Hubs
{
    public interface INotificationHub
    {
        public Task SendAsync(NotificationDto notification);
    }
}
