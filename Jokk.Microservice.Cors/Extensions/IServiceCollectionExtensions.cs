using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Jokk.Microservice.Cors.Extensions
{
    public static class IServiceCollectionExtensions
    {
        private const string PolicyName = "microservice";

        public static IServiceCollection AddMicroserviceCors(this IServiceCollection services, CorsConfiguration configuration)
        {
            services.AddSingleton(configuration);
            return services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    if (services.Any())
                        policy.WithOrigins(configuration.Services.ToArray());
                    else
                        throw new ArgumentException("Services must be non-empty", nameof(configuration));

                    if (configuration.Methods is null)
                        policy.AllowAnyMethod();
                    else
                        policy.WithMethods(configuration.Methods.ToArray());
                    
                    policy.AllowAnyHeader();
                });
            });
        }

        public static IApplicationBuilder UseMicroserviceCors(this IApplicationBuilder app)
        {
            return app.UseCors(PolicyName);
        }
    }
}
