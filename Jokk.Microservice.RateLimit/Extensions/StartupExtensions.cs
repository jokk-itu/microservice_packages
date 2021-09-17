using System.Collections.Generic;
using Jokk.Microservice.RateLimit.Concealed;
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
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(configuration["ConnectionString"]));
            services.AddSingleton(serviceProvider => RedLockFactory.Create(new List<RedLockMultiplexer>
            {
                new(serviceProvider.GetRequiredService<IConnectionMultiplexer>())
            }));
            
            services.AddTransient<DistributedIpContext>();
            return services;
        }

        public static IServiceCollection AddMicroserviceConcealedRateLimit(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddConfiguration(configuration);
            services.AddSingleton<ConcealedIpContext>();
            return services;
        }

        public static IApplicationBuilder UseMicroserviceDistributedRateLimit(this IApplicationBuilder app)
        {
            app.UseMiddleware<DistributedRateLimitMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseMicroserviceConcealedRateLimit(this IApplicationBuilder app)
        {
            app.UseMiddleware<ConcealedRateLimitMiddleware>();
            return app;
        }

        private static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(_ => new RateLimitConfiguration()
            {
                RedisConnectionString = configuration["ConnectionString"],
                DayMax = long.Parse(configuration["DayMax"]),
                HourMax = long.Parse(configuration["HourMax"]),
                MinuteMax = long.Parse(configuration["MinuteMax"])
            });
            return services;
        }
    }
}