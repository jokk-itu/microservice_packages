using System.Net.Http;
using Jokk.Microservice.Prometheus.Constants;
using Jokk.Microservice.Prometheus.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace Jokk.Microservice.Prometheus
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddPrometheus(this IServiceCollection services, string[] serviceUris, string sqlserver = null, string mongodb = null, IConfigurationSection neo4J = null)
        {
            AddNeo4J(services, neo4J);
            AddMongo(services, mongodb);
            AddSqlServer(services, sqlserver);
            AddServiceHealthChecks(services, serviceUris);
            services.AddHealthChecks().ForwardToPrometheus();
            return services;
        }

        private static void AddServiceHealthChecks(IServiceCollection services, string[] serviceUris)
        {
            services.AddTransient(serviceProvider => 
                new ServiceHealthCheck(serviceProvider.GetRequiredService<IHttpClientFactory>(), serviceUris));
            services.AddHealthChecks().AddCheck<ServiceHealthCheck>("service_health_check");
        }

        private static void AddNeo4J(IServiceCollection services, IConfigurationSection neo4J)
        {
            if (neo4J != null)
            {
                services.AddTransient(serviceProvider => 
                    new Neo4JHealthCheck(neo4J, serviceProvider.GetRequiredService<IHttpClientFactory>()));
                services.AddHealthChecks().AddCheck<Neo4JHealthCheck>("graph_health_check");
            }
        }

        private static void AddMongo(IServiceCollection services, string mongodb)
        {
            if (mongodb != null)
                services.AddHealthChecks().AddMongoDb(mongodb);
        }

        private static void AddSqlServer(IServiceCollection services, string sqlserver)
        {
            if (sqlserver != null)
                services.AddHealthChecks().AddSqlServer(sqlserver);
        }

        public static IApplicationBuilder UsePrometheus(this IApplicationBuilder app)
        {
            app.UseHttpMetrics();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks(HealthCheckEndpoint.Endpoint); //Add Authorization (Basic Auth?)
                endpoints.MapMetrics();
            });
            return app;
        }
    }
}