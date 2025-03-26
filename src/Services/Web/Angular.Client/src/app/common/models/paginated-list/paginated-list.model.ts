export class PaginatedList<T> {
    public Items: T[] = [];
    public TotalCount: number = 0;
    public PageIndex: number = 1;
    public PageSize: number = 10;
    public PagesCount: number = 1;
}