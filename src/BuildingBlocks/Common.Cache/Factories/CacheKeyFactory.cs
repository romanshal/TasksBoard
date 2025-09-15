using Common.Cache.Interfaces.Factories;

namespace Common.Cache.Factories
{
    public class CacheKeyFactory : ICacheKeyFactory
    {
        public string Key<T>(Guid id) => $"{typeof(T).Name}_{id}";
    }
}
