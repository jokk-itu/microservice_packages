using Microsoft.Extensions.DependencyInjection;
using Prometheus.SystemMetrics;

namespace Jokk.Microservice.Prometheus.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds SystemMetrics to the metrics endpoint
        /// Remember to add <code>.ForwardToPrometheus()</code> after adding HealthChecks.
        /// For example <example>services.AddHealthChecks().ForwardToPrometheus()</example>
        /// Remember to add <code>.UseHttpClientMetrics()</code> after add HttpClient.
        /// For example <example>services.AddHttpClient().UseHttpClientMetrics()</example>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMicroservicePrometheus(
            this IServiceCollection services)
        {
            services.AddSystemMetrics();
            return services;
        }
    }
}