using Common.gRPC.Interfaces.Services;

namespace TasksBoard.Application.Handlers
{
    public class UserProfileHandler(
        IUserProfileService profileService)
    {
        private readonly IUserProfileService _profileService = profileService;

        public async Task Handle<T>(
            T value,
            Func<T, Guid> accountIdSelector,
            Action<T, string> accountNameSetter,
            CancellationToken cancellationToken = default)
        {
            await Handle([value], accountIdSelector, accountNameSetter, cancellationToken);
        }

        public async Task Handle<T>(
            IEnumerable<T> values, 
            Func<T, Guid> accountIdSelector,
            Action<T, string> accountNameSetter,
            CancellationToken cancellationToken = default)
        {
            if (values is null || !values.Any()) return;

            var userIds = values
                .Select(accountIdSelector)
                .Where(id => id != Guid.Empty)
                .Distinct();

            var userProfiles = await _profileService.ResolveAsync(userIds, cancellationToken);

            if (userProfiles.Count > 0)
            {
                foreach (var value in values)
                {
                    var accountId = accountIdSelector(value);
                    if (userProfiles.TryGetValue(accountId, out var profile) && profile != null)
                    {
                        accountNameSetter(value, profile.Username);
                    }
                }
            }
        }

        public async Task Handle<T>(
            IEnumerable<T> values,
            Func<T, Guid> accountIdSelector,
            Action<T, string, string> accountProfileSetter,
            CancellationToken cancellationToken = default)
        {
            if (values is null || !values.Any()) return;

            var userIds = values
                .Select(accountIdSelector)
                .Where(id => id != Guid.Empty)
                .Distinct();

            var userProfiles = await _profileService.ResolveAsync(userIds, cancellationToken);

            if (userProfiles.Count > 0)
            {
                foreach (var value in values)
                {
                    var accountId = accountIdSelector(value);
                    if (userProfiles.TryGetValue(accountId, out var profile) && profile != null)
                    {
                        accountProfileSetter(value, profile.Username, profile.Email);
                    }
                }
            }
        }
    }
}
