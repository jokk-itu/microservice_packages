using System;
using System.Net.Sockets;
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
        public static IHostBuilder AddMicroserviceLogging(this IHostBuilder host)
        {
            return host.UseSerilog((builderContext, _, loggerConfig) =>
            {
                var logConfig = GetLogConfiguration(builderContext.Configuration);
                ValidateConfig(logConfig);
                SetMinimumLevel(loggerConfig);
                SetOverrideMinimumLevel(loggerConfig, logConfig);
                SetSinks(loggerConfig, logConfig);
                loggerConfig
                    .Enrich.FromLogContext()
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .Enrich.WithThreadId()
                    .Enrich.WithThreadName()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithSpan()
                    .Enrich.With<CorrelationIdEnricher>()
                    .Enrich.With<EnvironmentEnricher>()
                    .Enrich.WithProperty("Service", logConfig.Service);
            });
        }

        private static LogConfiguration GetLogConfiguration(IConfiguration appSettings)
        {
            var configuration = new LogConfiguration();
            appSettings.Bind("Logging", configuration);

            return configuration;
        }

        private static void SetMinimumLevel(LoggerConfiguration logConfig)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == null)
                throw new EnvironmentException("ASPNETCORE_ENVIRONMENT is not set");

            if (environment!.Equals(Environments.Development))
                logConfig.MinimumLevel.Debug();
            else
                logConfig.MinimumLevel.Warning();
        }

        private static void SetOverrideMinimumLevel(LoggerConfiguration loggerConfig, LogConfiguration logConfig)
        {
            foreach (var (name, url) in logConfig.Overrides)
            {
                var logEventLevel = Enum.Parse<LogEventLevel>(url);
                loggerConfig.MinimumLevel.Override(name, logEventLevel);
            }
        }

        private static void SetSinks(LoggerConfiguration loggerConfig, LogConfiguration logConfig)
        {
            if (logConfig.LogToSeq)
                loggerConfig.WriteTo.Seq(logConfig.SeqUrl);
            if (logConfig.LogToElasticSearch)
                loggerConfig.WriteTo.Elasticsearch(logConfig.ElasticSearchUrl);
            if (logConfig.LogToConsole)
                loggerConfig.WriteTo.Console();
            if (logConfig.LogToUdp)
                loggerConfig.WriteTo.Udp(logConfig.UdpHost, logConfig.UdpPort, AddressFamily.InterNetwork);
        }

        private static void ValidateConfig(LogConfiguration logConfig)
        {
            if (logConfig.LogToSeq && !Uri.IsWellFormedUriString(logConfig.SeqUrl, UriKind.Absolute))
                throw new ArgumentException($"{logConfig.SeqUrl} is ill formatted");
            
            if (logConfig.LogToElasticSearch && !Uri.IsWellFormedUriString(logConfig.ElasticSearchUrl, UriKind.Absolute))
                throw new ArgumentException($"{logConfig.ElasticSearchUrl} is ill formatted");
        }
    }
}