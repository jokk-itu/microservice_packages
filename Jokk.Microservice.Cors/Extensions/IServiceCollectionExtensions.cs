using System.Linq;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jokk.Microservice.Cors.Extensions
{
    public static class IServiceCollectionExtensions
    {
        private const string PolicyName = "microservice";

        public static IServiceCollection AddMicroserviceCors(this IServiceCollection services, CorsConfiguration configuration)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    policy.WithOrigins(configuration.Services.ToArray())
                        .AllowAnyHeader();
                    if (configuration.Methods is null)
                        policy.AllowAnyMethod();
                    else
                        policy.WithMethods(configuration.Methods.ToArray());
                });
            });
        }

        public static IApplicationBuilder UseMicroserviceCors(this IApplicationBuilder app)
        {
            return app.UseCors(PolicyName);
        }
    }
}
