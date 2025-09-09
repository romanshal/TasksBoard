namespace Common.Cache.Interfaces
{
    public interface ICacheRepository
    {
        Task CreateAsync<T>(string key, T entity);
        Task<T?> GetAsync<T>(string key);
        Task<IDictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys);
    }
}
