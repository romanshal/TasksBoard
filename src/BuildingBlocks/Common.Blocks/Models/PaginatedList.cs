namespace Common.Blocks.Models
{
    public class PaginatedList<T> where T : class
    {
        public IEnumerable<T> Items { get; private set; }
        public int PagesCount { get; private set; }
        public int TotalCount { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }

        private PaginatedList(IEnumerable<T> items, int pageIndex, int pagSize, int totalCount)
        {
            Items = items;
            PageIndex = pageIndex;
            PageSize = pagSize;

            if (items.Any() && totalCount == default)
            {
                TotalCount = items.Count();
            }
            else
            {
                TotalCount = totalCount;
            }

            PagesCount = (int)Math.Ceiling((double)TotalCount / PageSize);
        }

        public static PaginatedList<T> Create(
            IEnumerable<T> items,
            int pageIndex,
            int pageSize,
            int totalCount = default) => new(items, pageIndex, pageSize, totalCount);


        public static PaginatedList<T> Empty(
            int pageIndex,
            int pageSize) => new([], pageIndex, pageSize, 0);
    }
}
