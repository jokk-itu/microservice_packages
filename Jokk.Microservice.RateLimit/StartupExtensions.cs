using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Jokk.Microservice.RateLimit
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddMicroserviceRateLimiting(this IServiceCollection services, IConfiguration ipRateLimiting, IConfiguration ipRateLimitingPolicies, string redisConnectionString)
        {
            services.AddOptions();
            services.AddMemoryCache();
            
            services.Configure<IpRateLimitOptions>(ipRateLimiting);
            services.Configure<IpRateLimitPolicies>(ipRateLimitingPolicies);
            
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IConnectionMultiplexer>(_ => 
                ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(redisConnectionString)));
            services.AddRedisRateLimiting();

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            return services;
        }

        public static IApplicationBuilder UseMicroserviceRateLimiting(this IApplicationBuilder app)
        {
            app.UseIpRateLimiting();
            return app;
        }
    }
}