using System.Threading.Tasks;
using Jokk.Microservice.RateLimit.Exceptions;
using Jokk.Microservice.RateLimit.Extensions;
using Microsoft.AspNetCore.Http;

namespace Jokk.Microservice.RateLimit.Memory
{
    internal class MemoryRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly MemoryIpContext _context;

        public MemoryRateLimitMiddleware(RequestDelegate next, MemoryIpContext context)
        {
            _next = next;
            _context = context;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                if (_context.UpdateRate(httpContext.GetIpAddress()))
                    await _next(httpContext);
                else
                    httpContext.Response.StatusCode = 429;
            }
            catch (UpdateConcurrencyException)
            {
                httpContext.Response.StatusCode = 409;
            }
        }
    }
}