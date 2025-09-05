using Common.gRPC.Interfaces.Caches;
using Common.gRPC.Interfaces.Services;
using Common.gRPC.Models;
using Common.gRPC.Protos;

namespace Common.gRPC.Services
{
    public class UserProfileService(
        IUserProfileCacheRepository cache,
        UserProfiles.UserProfilesClient grpcClient) : IUserProfileService
    {
        public async Task<UserProfile?> ResolveAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var profiles = await ResolveAsync([id], cancellationToken);

            return profiles.GetValueOrDefault(id);
        }

        public async Task<IReadOnlyDictionary<Guid, UserProfile>> ResolveAsync(
            HashSet<Guid> ids,
            CancellationToken cancellationToken = default)
        {
            var found = new Dictionary<Guid, UserProfile>();

            if (ids.Count == 0) return found;

            var cached = await cache.GetManyAsync(ids);
            var misses = new List<Guid>();

            foreach (var id in ids)
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
                .ToHashSet();

            await cache.SetManyAsync(received);

            var receivedIds = received.Select(u => u.Id).ToHashSet();
            var missingAfterCall = misses.Where(id => !receivedIds.Contains(id)).ToHashSet();
            if (missingAfterCall.Count > 0)
                await cache.SetNegativeAsync(missingAfterCall);

            foreach (var u in received) found[u.Id] = u;

            return found;
        }
    }
}
