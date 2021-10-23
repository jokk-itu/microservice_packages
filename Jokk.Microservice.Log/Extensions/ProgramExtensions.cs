using System;
using System.Linq;
using System.Net.Sockets;
using Jokk.Microservice.Log.Enrichers;
using Jokk.Microservice.Log.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            return host.UseSerilog((_, serviceProvider, loggerConfig) =>
            {
                var logConfig = serviceProvider.GetRequiredService<LogConfiguration>();
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

        private static void SetMinimumLevel(LoggerConfiguration loggerConfig)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == null)
                throw new EnvironmentException("ASPNETCORE_ENVIRONMENT is not set");

            if (environment!.Equals(Environments.Development))
                loggerConfig.MinimumLevel.Debug();
            else
                loggerConfig.MinimumLevel.Warning();
        }

        private static void SetOverrideMinimumLevel(LoggerConfiguration loggerConfig, LogConfiguration logConfig)
        {
            if (logConfig.Overrides is null || !logConfig.Overrides.Any())
                return;
            
            foreach (var (name, level) in logConfig.Overrides)
            {
                var logEventLevel = Enum.Parse<LogEventLevel>(level);
                loggerConfig.MinimumLevel.Override(name, logEventLevel);
            }
        }

        private static void SetSinks(LoggerConfiguration loggerConfig, LogConfiguration logConfig)
        {
            if (logConfig.LogToSeq 
                && logConfig.SeqUrl is not null 
                && IsValidUri(logConfig.SeqUrl))
                loggerConfig.WriteTo.Seq(logConfig.SeqUrl);
            
            if (logConfig.LogToElasticSearch 
                && logConfig.ElasticSearchUrl is not null 
                && IsValidUri(logConfig.ElasticSearchUrl))
                loggerConfig.WriteTo.Elasticsearch(logConfig.ElasticSearchUrl);
            
            if (logConfig.LogToConsole)
                loggerConfig.WriteTo.Console();
            
            if (logConfig.LogToUdp && logConfig.UdpHost is not null)
                loggerConfig.WriteTo.Udp(logConfig.UdpHost, logConfig.UdpPort, AddressFamily.InterNetwork);
        }

        private static bool IsValidUri(string uri)
            => !Uri.IsWellFormedUriString(uri, UriKind.Absolute);
    }
}