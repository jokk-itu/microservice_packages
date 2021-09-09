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
            ValidateServices(configuration);
            ValidateMongo(configuration);
            ValidateNeo4J(configuration);
            
            AddNeo4J(services, configuration);
            AddMongo(services, configuration);
            AddSqlServer(services, configuration);
            AddServiceHealthChecks(services, configuration);
            
            services.AddHealthChecks().ForwardToPrometheus();
            services.AddHttpClient(ClientName.HealthCheck).UseHttpClientMetrics();
            services.AddSystemMetrics();
            return services;
        }

        private static void ValidateServices(PrometheusConfiguration configuration)
        {
            foreach (var (service, uri) in configuration.Services)
            {
                if (string.IsNullOrEmpty(service))
                    throw new ArgumentException($"{nameof(service)} has to be specified");
                
                if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                    throw new UriFormatException($"{uri} for {service} is not a correct Uri");
            }
        }

        private static void ValidateNeo4J(PrometheusConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(configuration.Neo4JUri)
                && !Uri.IsWellFormedUriString(configuration.Neo4JUri, UriKind.Absolute))
                throw new ArgumentException($"{configuration.Neo4JUri} is ill formed");
        }

        private static void ValidateMongo(PrometheusConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(configuration.MongoUri)
                && !Uri.IsWellFormedUriString(configuration.MongoUri, UriKind.Absolute))
                throw new ArgumentException($"{configuration.MongoUri} is ill formed");
        }

        private static void AddServiceHealthChecks(IServiceCollection services, PrometheusConfiguration configuration)
        {
            foreach (var (service, uri) in configuration.Services)
            {
                services.AddTransient(serviceProvider => 
                    new ServiceHealthCheck(serviceProvider.GetRequiredService<IHttpClientFactory>(), uri, service));
                services.AddHealthChecks().AddCheck<ServiceHealthCheck>(service);
            }
        }

        private static void AddNeo4J(IServiceCollection services, PrometheusConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.Neo4JUri)) 
                return;
            
            services.AddTransient(serviceProvider => 
                new Neo4JHealthCheck(configuration, serviceProvider.GetRequiredService<IHttpClientFactory>()));
            services.AddHealthChecks().AddCheck<Neo4JHealthCheck>(HealthCheckName.Neo4J);
        }

        private static void AddMongo(IServiceCollection services, PrometheusConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(configuration.MongoUri))
            {
                var connectionString = configuration.MongoUri.Split("://");
                connectionString.SetValue($"{configuration.MongoUsername}:{configuration.MongoPassword}@", 1);
                services.AddHealthChecks().AddMongoDb(connectionString.ToString());
            }
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