export class PaginatedList<T> {
    public items: T[] = [];
    public totalCount: number = 0;
    public pageIndex: number = 1;
    public pageSize: number = 10;
    public pagesCount: number = 1;
}