using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models.UserProfiles;

namespace TasksBoard.Application.Factories
{
    internal static class UserProfileMappingFactory
    {
        public static IUserProfileMapping Create<T>(
            IEnumerable<T> items,
            Func<T, Guid> idSelector,
            Action<T, string, string?> setter) where T : BaseDto
        {
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(idSelector);
            ArgumentNullException.ThrowIfNull(setter);

            return new UserProfileMapping(
                items.Cast<object>(),
                o => idSelector((T)o),
                (o, username, email) => setter((T)o, username, email)
            );
        }
    }
}
