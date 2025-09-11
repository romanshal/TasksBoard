namespace TasksBoard.Infrastructure.CacheBuffers
{
    internal interface IAsyncCacheTransactionBuffer
    {
        void AddPending(Func<Task> action);
        Task FlushAsync(CancellationToken cancellationToken = default);
        void Clear();
    }
}
