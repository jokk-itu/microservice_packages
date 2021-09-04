using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jokk.Microservice.Cors
{
    public static class Setup
    {
        private const string PolicyName = "microservice";

        public static IServiceCollection AddMicroserviceCors(this IServiceCollection services, IConfiguration hosts, IConfiguration methods = null)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    policy.WithOrigins(hosts.GetChildren().Select(host => host.Value).ToArray())
                        .AllowAnyHeader();
                    if (methods == null)
                        policy.AllowAnyMethod();
                    else
                        policy.WithMethods(methods.GetChildren().Select(method => method.Value).ToArray());
                });
            });
        }

        public static IApplicationBuilder UseMicroserviceCors(this IApplicationBuilder app)
        {
            return app.UseCors(PolicyName);
        }
    }
}
