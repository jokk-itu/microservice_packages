using Microsoft.Extensions.DependencyInjection;

namespace Jokk.Microservice.Swagger.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, SwaggerConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddApiVersioning(config => { config.ReportApiVersions = true;});
            services.AddVersionedApiExplorer(config =>
            {
                config.GroupNameFormat = "'v'VVV";
                config.SubstituteApiVersionInUrl = true;
            });
            services.AddSwaggerGen();
            services.ConfigureOptions<ConfigureSwaggerOptions>();
            return services;
        }
    }
}