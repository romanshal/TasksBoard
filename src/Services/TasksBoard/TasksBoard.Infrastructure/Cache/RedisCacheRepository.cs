using Common.Blocks.Configurations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using TasksBoard.Domain.Interfaces.Caches;

namespace TasksBoard.Infrastructure.Cache
{
    public class RedisCacheRepository(
        IConnectionMultiplexer multiplexer, 
        IOptions<CacheConfiguration> options) : ICacheRepository
    {
        private readonly IDatabase _db = multiplexer.GetDatabase();
        private readonly CacheConfiguration _conf = options.Value;

        public void Create<T>(string key, T entity)
        {
            var payload = JsonConvert.SerializeObject(entity);
            var optionsTtl = TimeSpan.FromSeconds(_conf.ExpirationTimeSeconds);

            _db.StringSet(key, payload, optionsTtl);
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
        {
            var cached = await _db.StringGetAsync(key);
            if (cached.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(cached!)!;
            }

            var result = await factory();

            var optionsTtl = TimeSpan.FromSeconds(_conf.ExpirationTimeSeconds);
            var payload = JsonConvert.SerializeObject(result);
            await _db.StringSetAsync(key, payload, optionsTtl);

            return result;
        }

        public void Update<T>(string key, T entity)
        {
            var cached = _db.StringGet(key);
            if(cached.HasValue)
            {
                Remove(key);
            }

            var payload = JsonConvert.SerializeObject(entity);
            var optionsTtl = TimeSpan.FromSeconds(_conf.ExpirationTimeSeconds);
            _db.StringSet(key, payload, optionsTtl);
        }

        public Task RemoveAsync(string key)
        {
            return _db.KeyDeleteAsync(key);
        }

        public void Remove(string key)
        {
            _db.KeyDelete(key);
        }
    }
}
