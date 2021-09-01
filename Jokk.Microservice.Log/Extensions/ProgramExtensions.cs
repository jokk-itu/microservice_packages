using System;
using System.Text.Json;
using Jokk.Microservice.Log.Enrichers;
using Jokk.Microservice.Log.Exceptions;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.Seq;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;

namespace Jokk.Microservice.Log.Extensions
{
    public static class ProgramExtensions
    {
        public static IHostBuilder AddLogging(this IHostBuilder host)
        {
            return host.UseSerilog((builderContext, services, logConfig) =>
            {
                SetMinimumLevel(logConfig);
                logConfig
                    .WriteTo.Seq(builderContext.Configuration["Logging:SeqUri"])
                    .Enrich.FromLogContext()
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .Enrich.WithThreadId()
                    .Enrich.WithThreadName()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithSpan()
                    .Enrich.With<CorrelationIdEnricher>();
            });
        }

        private static void SetMinimumLevel(LoggerConfiguration logConfig)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment!.Equals(Environments.Development))
                logConfig.MinimumLevel.Debug();
            else if (environment.Equals(Environments.Production))
                logConfig.MinimumLevel.Warning();
            else
                throw new EnvironmentException("ASPNETCORE_ENVIRONMENT is not set");
        }
    }
}