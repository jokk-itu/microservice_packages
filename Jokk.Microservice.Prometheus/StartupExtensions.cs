using System;
using System.Net.Http;
using Jokk.Microservice.Prometheus.Constants;
using Jokk.Microservice.Prometheus.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Prometheus.SystemMetrics;

namespace Jokk.Microservice.Prometheus
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddMicroservicePrometheus(
            this IServiceCollection services,
            PrometheusConfiguration configuration)
        {
            ValidateConfiguration(configuration);
            AddNeo4J(services, configuration);
            AddMongo(services, configuration);
            AddSqlServer(services, configuration);
            AddServiceHealthChecks(services, configuration);
            services.AddHealthChecks().ForwardToPrometheus();
            services.AddHttpClient(ClientName.HealthCheck).UseHttpClientMetrics();
            services.AddSystemMetrics();
            return services;
        }

        private static void ValidateConfiguration(PrometheusConfiguration configuration)
        {
            foreach (var (service, uri) in configuration.Services)
            {
                if (string.IsNullOrEmpty(service))
                    throw new ArgumentException($"{nameof(service)} has to be specified");
                
                if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                    throw new UriFormatException($"{uri} for {service} is not a correct Uri");
            }
        }

        private static void AddServiceHealthChecks(IServiceCollection services, PrometheusConfiguration configuration)
        {
            services.AddTransient(serviceProvider => 
                new ServiceHealthCheck(serviceProvider.GetRequiredService<IHttpClientFactory>(), configuration));
            services.AddHealthChecks().AddCheck<ServiceHealthCheck>("service_health_check");
        }

        private static void AddNeo4J(IServiceCollection services, PrometheusConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.Neo4JConnectionString)) 
                return;
            
            services.AddTransient(serviceProvider => 
                new Neo4JHealthCheck(configuration, serviceProvider.GetRequiredService<IHttpClientFactory>()));
            services.AddHealthChecks().AddCheck<Neo4JHealthCheck>("graph_health_check");
        }

        private static void AddMongo(IServiceCollection services, PrometheusConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(configuration.MongoConnectionString))
                services.AddHealthChecks().AddMongoDb(configuration.MongoConnectionString);
        }
        
        private static void AddSqlServer(IServiceCollection services, PrometheusConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(configuration.SqlServerConnectionString))
                services.AddHealthChecks().AddSqlServer(configuration.SqlServerConnectionString);
        }

        /// <summary>
        /// Called after <code>app.UseRouting()</code>
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMicroservicePrometheus(this IApplicationBuilder app)
        {
            app.UseHttpMetrics();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks(HealthCheckEndpoint.Endpoint);
                endpoints.MapMetrics();
            });
            return app;
        }
    }
}