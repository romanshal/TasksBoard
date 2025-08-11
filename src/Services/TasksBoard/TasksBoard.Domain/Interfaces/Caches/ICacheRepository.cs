namespace TasksBoard.Domain.Interfaces.Caches
{
    public interface ICacheRepository
    {
        void Create<T>(string key, T entity);
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);
        void Update<T>(string key, T entity);
        Task RemoveAsync(string key);
        void Remove(string key);
    }
}
