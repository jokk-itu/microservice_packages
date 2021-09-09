using System;
using Jokk.Microservice.Log.Enrichers;
using Jokk.Microservice.Log.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Exceptions;

namespace Jokk.Microservice.Log.Extensions
{
    public static class ProgramExtensions
    {
        public static IHostBuilder AddMicroserviceLogging(this IHostBuilder host, string serviceName)
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true)
                .AddEnvironmentVariables()
                .Build();

            var configuration = new LogConfiguration();
            appSettings.Bind("Logging", configuration);
            
            
            return host.UseSerilog((builderContext, services, logConfig) =>
            {
                ConfigureEnvironment(logConfig);
                SetOverrideMinimumLevel(logConfig, configuration);
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

        private static void SetOverrideMinimumLevel(LoggerConfiguration loggerConfig, LogConfiguration logConfiguration)
        {
            foreach (var (name, url) in logConfiguration.Overrides)
            {
                var logEventLevel = Enum.Parse<LogEventLevel>(url);
                loggerConfig.MinimumLevel.Override(name, logEventLevel);
            }
        }
    }
}