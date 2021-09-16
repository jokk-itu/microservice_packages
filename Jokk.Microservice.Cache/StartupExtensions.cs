using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Jokk.Microservice.Cache
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddMicroserviceCache(this IServiceCollection services)
        {
            services.AddResponseCaching(options =>
            {
                options.MaximumBodySize = 1000000;
                options.UseCaseSensitivePaths = true;
            });
            return services;
        }

        public static IApplicationBuilder UseMicroserviceCache(this IApplicationBuilder app)
        {
            app.UseResponseCaching();
            return app;
        }
    }
}