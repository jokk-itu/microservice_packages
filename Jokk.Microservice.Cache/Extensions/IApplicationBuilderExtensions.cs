using Microsoft.AspNetCore.Builder;

namespace Jokk.Microservice.Cache.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMicroserviceClientCache(this IApplicationBuilder app)
        {
            app.UseResponseCaching();
            return app;
        }
    }
}