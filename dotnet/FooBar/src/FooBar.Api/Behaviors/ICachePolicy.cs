using System;
using System.Linq;
using MediatR;

namespace FooBar.Api.Behaviors
{
    public interface ICachePolicy<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        DateTime? AbsoluteExpiration => null;
        TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(5);
        TimeSpan? SlidingExpiration => TimeSpan.FromSeconds(30);

        string GetCacheKey(TRequest request)
        {
            var r = new {request};
            var props = r.request.GetType().GetProperties().Select(pi => $"{pi.Name}:{pi.GetValue(r.request, null)}");
            return $"{typeof(TRequest).FullName}{{{string.Join(",", props)}}}";
        }
    }
}