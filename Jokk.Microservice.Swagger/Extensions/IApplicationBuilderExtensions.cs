using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;

namespace Jokk.Microservice.Swagger.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMicroserviceSwagger(this IApplicationBuilder app)
        {
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
            app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    foreach (var version in provider.ApiVersionDescriptions)
                    {
                        options.DisplayRequestDuration();
                        options.SwaggerEndpoint(
                            $"/swagger/{version.GroupName}/swagger.json",
                            version.GroupName.ToUpperInvariant());
                    }
                });
            return app;
        }
    }
}