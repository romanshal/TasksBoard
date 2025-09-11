using System;

namespace Common.Cache.Interfaces
{
    public interface ICacheRepository
    {
        Task<T?> GetAsync<T>(string key);
        Task<IDictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys);
        Task CreateAsync<T>(string key, T entity);
        Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> func);
        Task<bool> RemoveAsync(string key);
    }
}
