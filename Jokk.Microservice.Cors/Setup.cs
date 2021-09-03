using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Jokk.Microservice.Cors
{
    public static class Setup
    {
        private const string PolicyName = "microservice";

        public static IServiceCollection AddMicroserviceCors(this IServiceCollection services, string[] hosts)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    policy.WithOrigins(hosts)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }
        
        public static IServiceCollection AddMicroserviceCors(this IServiceCollection services, string[] hosts, string[] methods)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    policy.WithOrigins(hosts)
                        .AllowAnyHeader()
                        .WithMethods(methods);
                });
            });
        }

        public static IApplicationBuilder UseMicroserviceCors(this IApplicationBuilder app)
        {
            return app.UseCors(PolicyName);
        }
    }
}
