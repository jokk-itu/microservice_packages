using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Jokk.Microservice.HealthChecks
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddHealthChecks(this IServiceCollection services)
        {
            return services;
        }
        
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
        {
            return app;
        }
    }
}