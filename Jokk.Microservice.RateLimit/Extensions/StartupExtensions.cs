using System.Collections.Generic;
using Jokk.Microservice.RateLimit.Distributed;
using Jokk.Microservice.RateLimit.Memory;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Jokk.Microservice.RateLimit.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddMicroserviceDistributedRateLimit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguration(configuration);
            services.AddSingleton<IConnectionMultiplexer>(serviceProvider => 
                ConnectionMultiplexer.Connect(
                    serviceProvider.GetRequiredService<RateLimitConfiguration>().RedisConnectionString));
            
            services.AddSingleton(serviceProvider => RedLockFactory.Create(new List<RedLockMultiplexer>
            {
                new(serviceProvider.GetRequiredService<IConnectionMultiplexer>())
            }));
            
            services.AddTransient<DistributedIpContext>();
            return services;
        }

        public static IServiceCollection AddMicroserviceMemoryRateLimit(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddConfiguration(configuration);
            services.AddSingleton<MemoryIpContext>();
            return services;
        }

        public static IApplicationBuilder UseMicroserviceDistributedRateLimit(this IApplicationBuilder app)
        {
            app.UseMiddleware<DistributedRateLimitMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseMicroserviceMemoryRateLimit(this IApplicationBuilder app)
        {
            app.UseMiddleware<MemoryRateLimitMiddleware>();
            return app;
        }

        private static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var config = new RateLimitConfiguration();
            configuration.Bind(config);
            services.AddSingleton(_ => config);
            return services;
        }
    }
}