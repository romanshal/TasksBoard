using Notification.Domain.ValueObjects;

namespace Notification.Application.Extensions
{
    internal static class EnumerableExtension
    {
        public static IEnumerable<ApplicationEventId> ToValueObjectList(this IEnumerable<Guid> guids)
        {
            return guids.Select(ApplicationEventId.Of);
        }
    }
}
