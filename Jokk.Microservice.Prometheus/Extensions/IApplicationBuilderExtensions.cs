using Jokk.Microservice.Prometheus.Constants;
using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace Jokk.Microservice.Prometheus.Extensions
{
    public static class IApplicationBuilderExtensions
    {
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