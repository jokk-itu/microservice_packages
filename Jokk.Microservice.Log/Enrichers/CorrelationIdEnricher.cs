using Jokk.Microservice.Log.Constants;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Jokk.Microservice.Log.Enrichers
{
    internal class CorrelationIdEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string Event = "CorrelationId";

        public CorrelationIdEnricher() : this(new HttpContextAccessor()) {}

        private CorrelationIdEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var correlationId = httpContext?.Request.Headers[CorrelationId.Header].ToString();
            var property = propertyFactory.CreateProperty(Event, correlationId);
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}