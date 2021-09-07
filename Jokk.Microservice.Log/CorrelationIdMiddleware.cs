using System;
using System.Linq;
using System.Threading.Tasks;
using Jokk.Microservice.Log.Constants;
using Microsoft.AspNetCore.Http;

namespace Jokk.Microservice.Log
{
    internal class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            AddCorrelationId(httpContext);
            await _next(httpContext);
        }

        private void AddCorrelationId(HttpContext httpContext)
        {
            var correlationId = httpContext.Request.Headers[CorrelationId.Header];
            if (!correlationId.Any())
                httpContext.Request.Headers[CorrelationId.Header] = Guid.NewGuid().ToString();
        }
    }
}