using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Jokk.Microservice.Prometheus
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddPrometheus(this IServiceCollection services)
        {
            services.AddHealthChecks().ForwardToPrometheus();
            return services;
        }

        public static IApplicationBuilder UsePrometheus(this IApplicationBuilder app)
        {
            app.UseHttpMetrics();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
            });
            return app;
        }
    }
}