using Common.Hubs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Notification.API.Hubs
{
    [Authorize]
    public class NotificationHub(
        IConnectionService connectionService) : Hub<INotificationHub>
    {
        public override Task OnConnectedAsync()
        {
            if (TryExtractUserId(out var userId))
            {
                connectionService.Add(userId, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if(TryExtractUserId(out var userId))
            {
                connectionService.Remove(userId, Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        private bool TryExtractUserId(out Guid userId)
        {
            var identifier = Context?.UserIdentifier;
            if (string.IsNullOrWhiteSpace(identifier))
            {
                userId = Guid.Empty;
                return false;
            }

            return Guid.TryParse(identifier, out userId);
        }
    }
}
