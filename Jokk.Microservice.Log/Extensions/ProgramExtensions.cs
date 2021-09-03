using System;
using System.Text.Json;
using Jokk.Microservice.Log.Enrichers;
using Jokk.Microservice.Log.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.Seq;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;

namespace Jokk.Microservice.Log.Extensions
{
    public static class ProgramExtensions
    {
        public static IHostBuilder AddLogging(this IHostBuilder host, string serviceName, ConfigurationSection logSection)
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

        private static void SetOverrideMinimumLevel(LoggerConfiguration logConfig, ConfigurationSection logSection)
        {
            var overrides = logSection.GetSection("Overrides").GetChildren();
            foreach (var section in overrides)
            {
                //Set Override for namespace and its' LogLevel
            }
        }
    }
}