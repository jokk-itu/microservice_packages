using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Jokk.Microservice.RateLimit.Concealed
{
    internal class ConcealedRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ConcealedIpContext _context;

        public ConcealedRateLimitMiddleware(RequestDelegate next, ConcealedIpContext context)
        {
            _next = next;
            _context = context;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _next(httpContext);
        }
    }
}