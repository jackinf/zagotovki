using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FooBar.Api.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<ICachePolicy<TRequest, TResponse>> cachePolicies;
        private readonly IDistributedCache cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> logger;

        public CachingBehavior(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger, IEnumerable<ICachePolicy<TRequest, TResponse>> cachePolicies)
        {
            this.cache = cache;
            this.logger = logger;
            this.cachePolicies = cachePolicies;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var cachePolicy = cachePolicies.FirstOrDefault();
            if (cachePolicy == null)
            {
                // No cache policy found, so just continue through the pipeline
                return await next();
            }
            var cacheKey = cachePolicy.GetCacheKey(request);
            var cachedResponse = await cache.GetStringAsync(cacheKey, cancellationToken);
            if (cachedResponse != null)
            {
                logger.LogDebug($"Response retrieved {typeof(TRequest).FullName} from cache. CacheKey: {cacheKey}");
                var deserialized = JsonConvert.DeserializeObject<TResponse>(cachedResponse);
                return deserialized;
            }

            var response = await next();
            logger.LogDebug($"Caching response for {typeof(TRequest).FullName} with cache key: {cacheKey}");

            await cache.SetStringAsync(cacheKey, await Task.Factory.StartNew(() => JsonConvert.SerializeObject(response), cancellationToken), 
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = cachePolicy.SlidingExpiration,
                    AbsoluteExpiration = cachePolicy.AbsoluteExpiration,
                    AbsoluteExpirationRelativeToNow = cachePolicy.AbsoluteExpirationRelativeToNow
                },
                cancellationToken);
            
            return response;
        }
    }
    
}