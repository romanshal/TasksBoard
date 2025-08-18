using TasksBoard.Domain.Interfaces.Caches;
using TasksBoard.Domain.Models;

namespace TasksBoard.Infrastructure.Cache
{
    public class UserProfileCacheRepository(
        ICacheRepository cacheRepository) : IUserProfileCacheRepository
    {
        private static string Key(Guid id) => $"user:{id:N}";
        private static string NegativeKey(Guid id) => $"user:{id:N}:negative";

        public async Task SetManyAsync(IEnumerable<UserProfile> users)
        {
            var tasks = new List<Task>();
            foreach(var user in users)
            {
                tasks.Add(cacheRepository.CreateAsync(Key(user.Id), user));
            }

            await Task.WhenAll(tasks);
        }

        public async Task SetNegativeAsync(IEnumerable<Guid> missings)
        {
            var tasks = missings.Select(id => {
                var key = NegativeKey(id);
                return cacheRepository.CreateAsync(key, NegativeMarker.Instance);
            });

            await Task.WhenAll(tasks);
        }

        public async Task<IDictionary<Guid, UserProfile?>> GetManyAsync(IEnumerable<Guid> userIds)
        {
            var result = new Dictionary<Guid, UserProfile?>();
            if(!userIds.Any()) return result;

            var ids = userIds.Distinct().ToArray();
            var redisKeys = ids.Select(Key).ToArray();

            var cached = await cacheRepository.GetManyAsync<UserProfile>(redisKeys);

            var notFoundIds = new List<Guid>();
            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var key = redisKeys[i];

                if(cached.TryGetValue(key, out var profile) && profile is not null) result[id] = profile;
                else notFoundIds.Add(id);
            }

            if (notFoundIds.Count == 0) return result;

            var negativeKeys = notFoundIds.Select(NegativeKey).ToArray();
            var negativeMarkers = await cacheRepository.GetManyAsync<NegativeMarker>(negativeKeys);

            for(int i = 0; i < notFoundIds.Count; i++)
            {
                var id = notFoundIds[i];
                var nKey = NegativeKey(id);

                if(negativeMarkers.TryGetValue(nKey, out var marker) && marker is not null) result[id] = null;
            }

            return result;
        }
    }
}
