using System.Threading.Tasks;
using Jokk.Microservice.RateLimit.Extensions;
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