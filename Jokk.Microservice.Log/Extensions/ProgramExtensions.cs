using System;
using System.Text.Json;
using Jokk.Microservice.Log.Enrichers;
using Jokk.Microservice.Log.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.Seq;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;

namespace Jokk.Microservice.Log.Extensions
{
    public static class ProgramExtensions
    {
        public static IHostBuilder AddMicroserviceLogging(this IHostBuilder host, string serviceName, IConfiguration logSection)
        {
            return host.UseSerilog((builderContext, services, logConfig) =>
            {
                ConfigureEnvironment(logConfig);
                SetOverrideMinimumLevel(logConfig, logSection);
                logConfig
                    .WriteTo.Seq(builderContext.Configuration["Logging:SeqUri"])
                    .Enrich.FromLogContext()
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .Enrich.WithThreadId()
                    .Enrich.WithThreadName()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithSpan()
                    .Enrich.With<CorrelationIdEnricher>()
                    .Enrich.WithProperty("Service", serviceName);
            });
        }

        private static void ConfigureEnvironment(LoggerConfiguration logConfig)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == null)
                throw new EnvironmentException("ASPNETCORE_ENVIRONMENT is not set");
            
            if (environment!.Equals(Environments.Development))
                logConfig.MinimumLevel.Debug();
            else
                logConfig.MinimumLevel.Warning();

            logConfig.Enrich.WithProperty("Environment", environment);
        }

        private static void SetOverrideMinimumLevel(LoggerConfiguration logConfig, IConfiguration logSection)
        {
            var overrides = logSection.GetSection("Overrides").GetChildren();
            foreach (var section in overrides)
            {
                var logEventLevel = Enum.Parse<Serilog.Events.LogEventLevel>(section.Value);
                logConfig.MinimumLevel.Override(section.Key, logEventLevel);
            }
        }
    }
}