using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Jokk.Microservice.Log.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddMicroserviceLogging(this IServiceCollection services)
        {
            services.ConfigureAll<HttpClientFactoryOptions>(options =>
            {
                options.HttpMessageHandlerBuilderActions.Add(builder =>
                {
                    builder.AdditionalHandlers.Add(
                        new HttpClientDelegate(
                            builder.Services.GetService<ILogger<HttpClientDelegate>>(), 
                            builder.Services.GetService<IHttpContextAccessor>()));
                });
            });
            return services;
        }

        public static IApplicationBuilder UseMicroserviceLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging((options) =>
            {
                options.EnrichDiagnosticContext = (diagnosticsContext, httpContext) =>
                {
                    
                };
            });
            app.UseMiddleware<CorrelationIdMiddleware>();
            return app;
        }
    }
}