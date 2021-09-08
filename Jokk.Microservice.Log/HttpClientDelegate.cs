using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jokk.Microservice.Log.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Jokk.Microservice.Log
{
    internal class HttpClientDelegate : DelegatingHandler
    {
        private readonly ILogger<HttpClientDelegate> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpClientDelegate(ILogger<HttpClientDelegate> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if(httpContext is null)
                return await base.SendAsync(request, cancellationToken);
            
            request.Headers.Add(CorrelationId.Header, httpContext.Request.Headers[CorrelationId.Header].ToString());
            
            var watch = Stopwatch.StartNew();
            var response = await base.SendAsync(request, cancellationToken);
            watch.Stop();
            
            if(response.IsSuccessStatusCode)
                _logger.LogInformation("Request to {Service} returned {Response} within {Elapsed} ms", 
                    request.RequestUri!.Host, response.StatusCode, watch.ElapsedMilliseconds);
            else
                _logger.LogError("Request to {Service} returned {Response}, with {Reason}, within {Elapsed} ms", 
                    request.RequestUri!.Host, response.StatusCode, response.ReasonPhrase, watch.ElapsedMilliseconds);
            
            return response;
        }
    }
}