using Common.Hubs.Interfaces;
using System.Collections.Concurrent;

namespace Common.Hubs.Services
{
    public class ConnectionService : IConnectionService
    {
        //TODO: change to redis
        private static readonly ConcurrentDictionary<Guid, HashSet<string>> Connections = new();

        public async Task Add(Guid userId, string connectionId)
        {
            Connections.AddOrUpdate(
                userId,
                _ => [connectionId],
                (_, connectionIds) =>
                {
                    connectionIds.Add(connectionId);
                    return connectionIds;
                });
        }

        public async Task Remove(Guid userId, string connectionId)
        {
            if(Connections.TryGetValue(userId, out var connectionIds))
            {
                if (connectionIds.Count == 0)
                {
                    Connections.TryRemove(userId, out _);
                }
            }
   
        }

        public IReadOnlyDictionary<Guid, IEnumerable<string>> GetConnectedUsers() => 
            Connections.ToDictionary(
                k => k.Key,
                v => v.Value.Select(c => c));
    }
}
