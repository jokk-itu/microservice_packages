using System;
using System.Net.Http;
using Jokk.Microservice.HealthCheck.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;

namespace Jokk.Microservice.HealthCheck.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IHealthChecksBuilder AddServiceHealthCheck(this IServiceCollection services, string name,
            Uri uri)
        {
            services.AddTransient(serviceProvider =>
                new ServiceHealthCheck(serviceProvider.GetRequiredService<IHttpClientFactory>(), name, uri));
            return services.AddHealthChecks().AddCheck<ServiceHealthCheck>(name);
        }

        public static IHealthChecksBuilder AddNeo4JHealthCheck(this IServiceCollection services, string healthCheckName)
        {
            services.AddTransient(serviceProvider =>
                new Neo4JHealthCheck(serviceProvider.GetRequiredService<IDriver>()));
            return services.AddHealthChecks().AddCheck<Neo4JHealthCheck>(healthCheckName);
        }

        public static IHealthChecksBuilder AddMongoHealthCheck(this IServiceCollection services, string connectionString)
        {
            return services.AddHealthChecks().AddMongoDb(connectionString);
        }
        
        public static IHealthChecksBuilder AddSqlServerHealthCheck(this IServiceCollection services, string connectionString)
        {
            return services.AddHealthChecks().AddSqlServer(connectionString);
        }
        
        public static IHealthChecksBuilder AddRedisHealthCheck(this IServiceCollection services, string connectionString)
        {
            return services.AddHealthChecks().AddRedis(connectionString);
        }
        
        public static IHealthChecksBuilder AddRabbitMqHealthCheck(this IServiceCollection services)
        {
            return services.AddHealthChecks().AddRabbitMQ();
        }
    }
}