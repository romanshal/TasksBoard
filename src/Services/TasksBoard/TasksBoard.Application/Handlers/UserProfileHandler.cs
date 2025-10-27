using Common.gRPC.Interfaces.Services;
using Common.gRPC.Models;
using TasksBoard.Application.Models.UserProfiles;

namespace TasksBoard.Application.Handlers
{
    internal class UserProfileHandler(
        IUserProfileService profileService) : IUserProfileHandler
    {
        private readonly IUserProfileService _profileService = profileService;

        public async Task Handle<T>(
            T value,
            Func<T, Guid> accountIdSelector,
            Action<T, string, string?> accountNameSetter,
            CancellationToken cancellationToken = default)
        {
            await Handle([value], accountIdSelector, accountNameSetter, cancellationToken);
        }

        public async Task Handle<T>(
            IEnumerable<T> values,
            Func<T, Guid> accountIdSelector,
            Action<T, string, string?> accountProfileSetter,
            CancellationToken cancellationToken = default)
        {
            if (values is null) return;

            var userIds = values
                .Select(accountIdSelector)
                .Where(id => id != Guid.Empty)
                .ToHashSet();

            if (userIds.Count == 0) return;

            var profiles = await _profileService.ResolveAsync(userIds, cancellationToken);

            SeedProfileValues(values, profiles, accountIdSelector, accountProfileSetter);
        }

        public async Task HandleMany(
            IEnumerable<IUserProfileMapping> mappings,
            CancellationToken cancellationToken = default)
        {
            if (mappings == null) return;

            var allIds = mappings
                .Where(m => m.Items != null)
                .SelectMany(MappId)
                .ToHashSet();

            if (allIds.Count == 0) return;

            var profiles = await _profileService.ResolveAsync(allIds, cancellationToken);

            foreach (var (items, idSelector, setter) in mappings) 
            {
                if (items == null) continue; 

                SeedProfileValues(items, profiles, idSelector, setter); 
            }
        }

        private static void SeedProfileValues<T>(
            IEnumerable<T> values,
            IReadOnlyDictionary<Guid, UserProfile> profiles,
            Func<T, Guid> idSelector,
            Action<T, string, string?> setter)
        {
            foreach (var value in values)
            {
                var id = idSelector(value);
                if (profiles.TryGetValue(id, out var profile))
                {
                    setter(value, profile.Username, profile.Email);
                }
            }
        }

        private static IEnumerable<Guid> MappId(IUserProfileMapping map) =>
            map.Items
            .Select(m => map.IdSelector(m))
            .Where(id => id != Guid.Empty);
    }
}
