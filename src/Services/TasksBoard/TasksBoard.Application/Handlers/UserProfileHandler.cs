using Common.gRPC.Interfaces.Services;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Handlers
{
    public class UserProfileHandler(
        IUserProfileService profileService)
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

            var list = values as IList<T> ?? [.. values];
            if (list.Count == 0) return;

            var userIds = values
                .Select(accountIdSelector)
                .Where(id => id != Guid.Empty)
                .ToHashSet();

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

        public async Task HandleMany(
            IEnumerable<IMapping> mappings,
            CancellationToken cancellationToken = default)
        {
            if (mappings == null) return;

            //var allIds = mappings
            //    .Where(m => m.Items != null)
            //    .SelectMany(m => m.Items)
            //    .Select(item => MappId(item, mappings))
            //    .Where(id => id != Guid.Empty)
            //    .ToHashSet();            

            var allIds = mappings
                .Where(m => m.Items != null)
                .SelectMany(MappId)
                .ToHashSet();

            if (allIds.Count == 0) return;

            var profiles = await _profileService.ResolveAsync(allIds, cancellationToken);

            foreach (var (items, idSelector, setter) in mappings)
            {
                if (items == null) continue;

                foreach (var item in items)
                {
                    var id = idSelector(item);
                    if (profiles.TryGetValue(id, out var profile))
                    {
                        setter(item, profile.Username, profile.Email);
                    }
                }
            }
        }

        //private static Guid MappId(object item, IEnumerable<IMapping> maps) => maps.First(m => m.Items.Contains(item)).IdSelector(item);
        private static IEnumerable<Guid> MappId(IMapping map) => map.Items.Select(m => map.IdSelector(m)).Where(id => id != Guid.Empty);

    }

    public interface IMapping
    {
        IEnumerable<object> Items { get; }
        Func<object, Guid> IdSelector { get; }
        Action<object, string, string?> Setter { get; }

        void Deconstruct(out IEnumerable<object> items, out Func<object, Guid> idSelector, out Action<object, string, string?> setter);
    }

    public record MappingImpl(
        IEnumerable<object> Items,
        Func<object, Guid> IdSelector,
        Action<object, string, string?> Setter) : IMapping
    {
        public void Deconstruct(out IEnumerable<object> items, out Func<object, Guid> idSelector, out Action<object, string, string?> setter)
        {
            items = Items;
            idSelector = IdSelector;
            setter = Setter;
        }
    }

    public static class MappingFactory
    {
        public static IMapping Create<T>(
            IEnumerable<T> items,
            Func<T, Guid> idSelector,
            Action<T, string, string?> setter) where T: BaseDto
        {
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(idSelector);
            ArgumentNullException.ThrowIfNull(setter);

            return new MappingImpl(
                items.Cast<object>(),
                o => idSelector((T)o),
                (o, username, email) => setter((T)o, username, email)
            );
        }
    }
}
