using Microsoft.AspNetCore.SignalR;

namespace Notification.Application.Providers
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.GetHttpContext()?.Request.Query["accountId"];
        }
    }
}
