using Common.gRPC.Interfaces.Services;
using Notification.Application.Dtos;
using Notification.Domain.Constants;

namespace Notification.Application.Handlers
{
    public class UserProfileHandler(
        IUserProfileService profileService)
    {

        public async Task Handle(
            NotificationDto notification, 
            CancellationToken cancellationToken = default)
        {
            await Handle([notification], cancellationToken);
        }

        public async Task Handle(
            IEnumerable<NotificationDto> notifications,
            CancellationToken cancellationToken = default)
        {
            var ids = new HashSet<Guid>();
            foreach (var notification in notifications)
            {
                var accountIdExist = notification.Payload.TryGetValue(NotificationLinkTypes.AccountId, out var accountId);
                var sourceAccountIdExist = notification.Payload.TryGetValue(NotificationLinkTypes.SourceAccountId, out var sourceAccountId);

                if (accountIdExist && !string.IsNullOrWhiteSpace(accountId)) ids.Add(Guid.Parse(accountId));
                if (sourceAccountIdExist && !string.IsNullOrWhiteSpace(sourceAccountId)) ids.Add(Guid.Parse(sourceAccountId));
            }

            if (ids.Count == 0) return;

            var userProfiles = await profileService.ResolveAsync(ids, cancellationToken);
            if (userProfiles.Count == 0) return;

            foreach (var notification in notifications)
            {
                if (notification.Payload.TryGetValue(NotificationLinkTypes.AccountId, out var accountId))
                {
                    var isExist = userProfiles.TryGetValue(Guid.Parse(accountId), out var profile);
                    if (isExist && profile is not null)
                    {
                        notification.Payload.Add(NotificationLinkTypes.AccountName, profile.Username);
                    }
                }

                if (notification.Payload.TryGetValue(NotificationLinkTypes.SourceAccountId, out var sourceAccountId))
                {
                    var isExist = userProfiles.TryGetValue(Guid.Parse(sourceAccountId), out var profile);
                    if (isExist && profile is not null)
                    {
                        notification.Payload.Add(NotificationLinkTypes.SourceAccountName, profile.Username);
                    }
                }
            }
        }
    }
}
