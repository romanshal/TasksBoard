using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace Chat.API.Hubs
{
    [Authorize]
    public class BoardChatHub : Hub
    {
        //<userId, HashSet<ConnectionId>>
        private static readonly ConcurrentDictionary<string, HashSet<string>> connectedUsers = new();

        //<ConnectionId, HashSet<boardId>>
        private static readonly ConcurrentDictionary<string, HashSet<string>> connectedBoards = new();

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

        public async Task JoinBoard(string boardId)
        {
            if (connectedBoards.TryGetValue(Context.ConnectionId, out var boardIds) && boardIds.Contains(boardId)) return;

            connectedBoards.AddOrUpdate(Context.ConnectionId,
                _ => [Context.ConnectionId],
                (_, boardIds) =>
                {
                    boardIds.Add(boardId);
                    return boardIds;
                });

            await Groups.AddToGroupAsync(Context.ConnectionId, boardId);
        }

        public async Task LeaveBoard(string boardId)
        {
            if (!connectedBoards.TryGetValue(Context.ConnectionId, out var boardIds) || !boardIds.Remove(boardId))
                return;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId);

            if (boardIds.Count == 0)
                connectedBoards.TryRemove(Context.ConnectionId, out _);
        }

        private string? ExtractUserId()
        {
            return Context?.User?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
