using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using Common.Cache.Interfaces;
using Common.Cache.Interfaces.Factories;

namespace Common.Cache.CQRS
{
    public interface ICachebleQuery<TType, TResponse> : IQuery<TResponse>, ICacheable where TResponse : Result
    {
        Guid Id { get; }
        string ICacheable.CacheKey(ICacheKeyFactory cacheKey) => cacheKey.Key<TType>(Id);
    }
}
