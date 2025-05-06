using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Hubs
{
    [Authorize]
    public class BoardChatHub : Hub
    {
        private static Dictionary<string, string> connections = [];

        public async Task JoinBoard(string boardId, string userId)
        {
            var user = Context.User;
            await Groups.AddToGroupAsync(Context.ConnectionId, boardId);

            var t = Groups;
        }

        public async Task LeaveBoard(string boardId, CancellationToken cancellationToken = default)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId, cancellationToken);
        }
    }
}
