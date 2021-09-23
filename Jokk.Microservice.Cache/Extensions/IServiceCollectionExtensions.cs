using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jokk.Microservice.Cache.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMicroserviceDistributedCache(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["ConnectionString"];
                options.InstanceName = "DistributedRedisCache";
            });
            return services;
        }
        
        public static IServiceCollection AddMicroserviceMemoryCache(
            this IServiceCollection services)
        {
            services.AddMemoryCache();
            return services;
        }

        public static IServiceCollection AddMicroserviceClientCache(this IServiceCollection services)
        {
            services.AddResponseCaching();
            return services;
        }
    }
}