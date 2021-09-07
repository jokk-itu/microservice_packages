using System.Net.Http;
using Jokk.Microservice.Prometheus.Constants;
using Jokk.Microservice.Prometheus.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using Prometheus.SystemMetrics;

namespace Jokk.Microservice.Prometheus
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddMicroservicePrometheus(
            this IServiceCollection services, 
            IConfigurationSection serviceUris, 
            string sqlserver = null, 
            string mongodb = null, 
            IConfigurationSection neo4J = null)
        {
            AddNeo4J(services, neo4J);
            AddMongo(services, mongodb);
            AddSqlServer(services, sqlserver);
            AddServiceHealthChecks(services, serviceUris);
            services.AddHealthChecks().ForwardToPrometheus();
            services.AddHttpClient(Options.DefaultName).UseHttpClientMetrics();
            services.AddSystemMetrics();
            return services;
        }

        private static void AddServiceHealthChecks(IServiceCollection services, IConfigurationSection serviceUris)
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
                //Add Authorization (Basic Auth?)
                endpoints.MapHealthChecks(HealthCheckEndpoint.Endpoint);
                endpoints.MapMetrics();
            });
            return app;
        }
    }
}