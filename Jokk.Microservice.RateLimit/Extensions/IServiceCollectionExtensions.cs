using System;
using System.Collections.Generic;
using System.Net;
using Jokk.Microservice.RateLimit.Distributed;
using Jokk.Microservice.RateLimit.Memory;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Jokk.Microservice.RateLimit.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMicroserviceDistributedRateLimit(this IServiceCollection services, RateLimitConfiguration configuration)
        {
            services.AddConfiguration(configuration);
            services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
                ConnectionMultiplexer.Connect(new ConfigurationOptions
                        {
                            ConnectRetry = 3,
                            EndPoints =
                            {
                                new DnsEndPoint(
                                    configuration.Host,
                                    configuration.Port)
                            },
                            Password = serviceProvider.GetRequiredService<RateLimitConfiguration>().Password
                        }
                    ));
            
            services.AddSingleton(serviceProvider => RedLockFactory.Create(new List<RedLockMultiplexer>
            {
                new(serviceProvider.GetRequiredService<IConnectionMultiplexer>())
            }));
            
            services.AddTransient<DistributedIpContext>();
            return services;
        }

        public static IServiceCollection AddMicroserviceMemoryRateLimit(this IServiceCollection services,
            RateLimitConfiguration configuration)
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

        private static void AddConfiguration(this IServiceCollection services, RateLimitConfiguration configuration)
        {
            if (configuration.Host is null || configuration.Password is null)
                throw new ArgumentException("Host and Password must be set", nameof(configuration));
            services.AddSingleton(configuration);
        }
    }
}