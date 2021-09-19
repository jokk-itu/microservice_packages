using System;
using Serilog.Core;
using Serilog.Events;

namespace Jokk.Microservice.Log.Enrichers
{
    public class EnvironmentEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment is null)
                return;
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Environment", environment));
        }
    }
}