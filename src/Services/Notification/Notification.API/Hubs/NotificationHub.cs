using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace Notification.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> connectedUsers = new();

        public override Task OnConnectedAsync()
        {
            var userId = ExtractUserId();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                connectedUsers.AddOrUpdate(userId,
                    _ => [Context.ConnectionId],
                    (_, connectionIds) =>
                    {
                        connectionIds.Add(Context.ConnectionId);
                        return connectionIds;
                    });
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = ExtractUserId();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                connectedUsers.TryGetValue(userId, out var connectionIds);
                connectionIds?.Remove(Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        private string? ExtractUserId()
        {
            return Context?.User?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
