using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Notification.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
