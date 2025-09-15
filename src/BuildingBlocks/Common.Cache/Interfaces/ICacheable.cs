using Common.Cache.Interfaces.Factories;

namespace Common.Cache.Interfaces
{
    public interface ICacheable
    {
        string CacheKey(ICacheKeyFactory cacheKey);
    }
}
