namespace TasksBoard.Infrastructure.CacheBuffers
{
    internal class AsyncCacheTransactionBuffer : IAsyncCacheTransactionBuffer
    {
        private readonly List<Func<Task>> _pending = [];

        public void AddPending(Func<Task> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            _pending.Add(action);
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default)
        {
            foreach (var action in _pending)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await action();
            }

            _pending.Clear();
        }

        public void Clear() => _pending.Clear();
    }
}
