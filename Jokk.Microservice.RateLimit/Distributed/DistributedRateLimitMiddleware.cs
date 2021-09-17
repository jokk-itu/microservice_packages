using System.Threading.Tasks;
using Jokk.Microservice.RateLimit.Extensions;
using Microsoft.AspNetCore.Http;

namespace Jokk.Microservice.RateLimit
{
    internal class DistributedRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DistributedIpContext _context;

        public DistributedRateLimitMiddleware(RequestDelegate next, DistributedIpContext context)
        {
            _next = next;
            _context = context;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var hasReachedLimit = await _context.UpdateRateAsync(httpContext.GetIpAddress());
            if (hasReachedLimit)
                httpContext.Response.StatusCode = 429;
            else
                await _next(httpContext);
        }
    }
}