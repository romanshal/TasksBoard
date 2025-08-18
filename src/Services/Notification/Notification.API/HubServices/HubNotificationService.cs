using Microsoft.AspNetCore.SignalR;
using Notification.API.Hubs;
using Notification.Application.Dtos;
using Notification.Application.Interfaces.HubServices;

namespace Notification.API.HubServices
{
    public class HubNotificationService(
        IHubContext<NotificationHub> hubContext) : IHubNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;

        public async Task NotifyUserAsync(string accountId, NotificationDto notification, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.User(accountId).SendAsync("ReceiveNotification", notification, cancellationToken);
        }
    }
}
