using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Jokk.Microservice.Cache.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMicroserviceDistributedCache(
            this IServiceCollection services,
            CacheConfiguration configuration)
        {
            ValidateConfig(configuration);
            services.AddTransient<ICacheStore, CacheStore>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions.EndPoints.Add(
                    new DnsEndPoint(configuration.Host, configuration.Port));
                options.ConfigurationOptions.Password = configuration.Password;
                options.ConfigurationOptions.ConnectRetry = 3;
            });
            return services;
        }

        private static void ValidateConfig(CacheConfiguration configuration)
        {
            if (configuration.Host is null || configuration.Password is null)
                throw new ArgumentException("Host and Password must be set", nameof(configuration));
        }

        public static IServiceCollection AddMicroserviceMemoryCache(
            this IServiceCollection services)
        {
            services.AddTransient<ICacheStore, CacheStore>();
            services.AddDistributedMemoryCache();
            return services;
        }

        public static IServiceCollection AddMicroserviceClientCache(this IServiceCollection services)
        {
            services.AddResponseCaching();
            return services;
        }
    }
}