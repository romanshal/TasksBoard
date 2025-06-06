﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Notification.Application.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
