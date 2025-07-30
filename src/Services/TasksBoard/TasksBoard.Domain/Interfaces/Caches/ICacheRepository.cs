namespace TasksBoard.Domain.Interfaces.Caches
{
    public interface ICacheRepository
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);
        Task RemoveAsync(string key);
    }
}
