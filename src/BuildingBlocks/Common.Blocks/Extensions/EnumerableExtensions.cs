using Common.Blocks.Models;

namespace Common.Blocks.Extensions
{
    public static class EnumerableExtensions
    {
        public static PaginatedList<T> ToPaginatedList<T>(this IEnumerable<T> list, int pageIndex = 1, int pageSize = 1, int totalCount = default) where T : class
        {
            if (list == null || !list.Any())
            {
                return new PaginatedList<T>([], pageIndex, pageSize, 0);
            }

            if (list.Any() && totalCount == default)
            {
                return new PaginatedList<T>(list, pageIndex, pageSize, list.Count());
            }

            return new PaginatedList<T>(list, pageIndex, pageSize, totalCount);
        }
    }
}
