using Common.Cache.Configurations;
using Common.Cache.Interfaces.Repositories;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Common.Cache.Repositories
{
    public class RedisCacheRepository(
        IConnectionMultiplexer multiplexer,
        IOptions<CacheConfiguration> options) : ICacheRepository
    {
        private readonly IDatabase _db = multiplexer.GetDatabase();
        private readonly CacheConfiguration _conf = options.Value;

        public Task CreateAsync<T>(string key, T entity)
        {
            var payload = JsonConvert.SerializeObject(entity);
            var optionsTtl = TimeSpan.FromSeconds(_conf.ExpirationTimeSeconds);

            return _db.StringSetAsync(key, payload, optionsTtl);
        }

        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> func)
        {
            var cached = await _db.StringGetAsync(key);
            if (cached.HasValue)
                return JsonConvert.DeserializeObject<T>(cached!);

            var value = await func();

            var payload = JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            var optionsTtl = TimeSpan.FromSeconds(_conf.ExpirationTimeSeconds);
            await _db.StringSetAsync(key, payload, optionsTtl);

            return value;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cached = await _db.StringGetAsync(key);
            if (cached.IsNullOrEmpty)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(cached!);
        }

        public async Task<IDictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Distinct().Select(k => (RedisKey)k).ToArray();
            if (redisKeys.Length == 0)
                return new Dictionary<string, T?>();

            var values = await _db.StringGetAsync(redisKeys);

            var result = new Dictionary<string, T?>(redisKeys.Length);
            for (int i = 0; i < redisKeys.Length; i++)
            {
                var value = values[i];
                if (value.IsNullOrEmpty)
                {
                    result[(string)redisKeys[i]!] = default;
                    continue;
                }

                result[(string)redisKeys[i]!] = JsonConvert.DeserializeObject<T>(value!);
            }

            return result;
        }

        public async Task<bool> RemoveAsync(string key)
        {
            return await _db.KeyDeleteAsync(key);
        }
    }
}
