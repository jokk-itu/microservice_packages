using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Jokk.Microservice.RateLimit.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddMicroserviceRateLimit(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
            services.AddSingleton(serviceProvider => RedLockFactory.Create(new List<RedLockMultiplexer>
            {
                new(serviceProvider.GetRequiredService<IConnectionMultiplexer>())
            }));
            
            services.AddTransient<IpContext>();
            return services;
        }

        public static IApplicationBuilder UseMicroserviceRateLimit(this IApplicationBuilder app)
        {
            app.UseMiddleware<RateLimitMiddleware>();
            return app;
        }
    }
}