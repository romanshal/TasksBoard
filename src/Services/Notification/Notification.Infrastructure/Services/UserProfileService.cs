using Common.Blocks.Interfaces.Caches;
using Common.Blocks.Interfaces.Services;
using Common.Blocks.Models.Users;
using Common.Blocks.Protos;

namespace Notification.Infrastructure.Services
{
    public class UserProfileService(
        IUserProfileCacheRepository cache,
        UserProfiles.UserProfilesClient grpcClient
        ) : IUserProfileService
    {
        public async Task<UserProfile?> ResolveAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var profiles = await ResolveAsync([id], cancellationToken);

            return profiles.GetValueOrDefault(id);
        }

        public async Task<IReadOnlyDictionary<Guid, UserProfile>> ResolveAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken = default)
        {
            var found = new Dictionary<Guid, UserProfile>();

            var distinct = ids.Where(id => id != Guid.Empty).Distinct();
            if (!distinct.Any()) return found;

            var cached = await cache.GetManyAsync(distinct);
            var misses = new List<Guid>();

            foreach (var id in distinct)
            {
                if (cached.TryGetValue(id, out var profile))
                {
                    if (profile is not null) found[id] = profile;
                }
                else
                {
                    misses.Add(id);
                }
            }

            if (misses.Count == 0) return found;

            var request = new ResolveUsersRequest();
            request.UserIds.AddRange(misses.Select(s => s.ToString()));
            ResolveUsersResponse response;
            try
            {
                response = await grpcClient.ResolveUsersAsync(request, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                return found;
            }

            var received = response.Users
                .Select(user => new UserProfile(Guid.Parse(user.Id), user.Username, user.Email))
                .ToArray();

            await cache.SetManyAsync(received);

            var receivedIds = received.Select(u => u.Id).ToHashSet();
            var missingAfterCall = misses.Where(id => !receivedIds.Contains(id)).ToArray();
            if (missingAfterCall.Length > 0)
                await cache.SetNegativeAsync(missingAfterCall);

            foreach (var u in received) found[u.Id] = u;

            return found;
        }
    }
}
