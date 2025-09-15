using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using Common.Cache.Interfaces;
using Common.Cache.Interfaces.Factories;
using Common.Cache.Interfaces.Repositories;
using MediatR;

namespace Common.Cache.Behaviours
{
    public class CacheBehaviour<TRequest, TResponse>(
        ICacheRepository cache,
        ICacheKeyFactory cacheKey) : IPipelineBehavior<TRequest, TResponse> where TRequest : IQuery<TResponse>, ICacheable where TResponse : Result
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var key = request.CacheKey(cacheKey);

            var cached = await cache.GetAsync<TResponse>(key);
            if (cached is not null)
            {
                return cached;
            }

            var response = await next(cancellationToken);

            if (response.IsSuccess)
            {
                try
                {
                    await cache.CreateAsync(key, response);
                }
                catch { }
            }

            return response;
        }
    }
}
