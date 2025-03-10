namespace Common.Blocks.Models
{
    public class PaginatedList<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }

        public PaginatedList(IEnumerable<T> items, int pageIndex, int pagSize, int totalCount = default)
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
        }
    }
}
