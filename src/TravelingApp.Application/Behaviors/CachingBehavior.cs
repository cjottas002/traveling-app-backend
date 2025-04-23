using MediatR;
using TravelingApp.Application.Abstractions;

namespace TravelingApp.Application.Behaviors
{
    public class CachingBehavior<TRequest, TResponse>(ICacheService cacheService)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not ICacheableQuery cacheableQuery)
                return await next();

            var cached = await cacheService.GetAsync<TResponse>(cacheableQuery.CacheKey);
            if (cached is not null)
                return cached;

            var response = await next();

            await cacheService.SetAsync(
                cacheableQuery.CacheKey,
                response,
                cacheableQuery.SlidingExpirationMinutes,
                cacheableQuery.AbsoluteExpirationMinutes);

            return response;
        }
    }
}
