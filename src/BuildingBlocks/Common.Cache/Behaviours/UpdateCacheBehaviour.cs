using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;
using Common.Cache.Interfaces;
using Common.Cache.Interfaces.Factories;
using Common.Cache.Interfaces.Repositories;
using MediatR;

namespace Common.Cache.Behaviours
{
    public class UpdateCacheBehaviour<TRequest, TResponse>(
        ICacheRepository cache,
        ICacheKeyFactory cacheKey) : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand<TResponse>, ICacheable where TResponse : Result
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next(cancellationToken);

            var key = request.CacheKey(cacheKey);

            await cache.CreateAsync(key, response);

            return response;
        }
    }
}
