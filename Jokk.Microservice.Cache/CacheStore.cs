using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Jokk.Microservice.Cache
{
    public class CacheStore : ICacheStore
    {
        private readonly IDistributedCache _cacheStore;
        private readonly ILogger<CacheStore> _logger;

        public CacheStore(IDistributedCache cacheStore, ILogger<CacheStore> logger)
        {
            _cacheStore = cacheStore;
            _logger = logger;
        }

        public async Task<T> GetValueAsync<T>(HttpContext httpContext, CancellationToken cancellationToken = default)
        {
            var value = await _cacheStore.GetStringAsync(httpContext.Request.Path, token: cancellationToken);
            _logger.LogDebug("Got value {} by key {}", value, httpContext.Request.Path);
            var json = JsonSerializer.Deserialize<T>(value);
            return json ?? throw new ArgumentException(
                "The fetched value is not serializable to type T", typeof(T).ToString());
        }

        public async Task AddValue(HttpContext httpContext, object value, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);
            await _cacheStore.SetStringAsync(httpContext.Request.Path, json, cancellationToken);
            _logger.LogDebug("Set value {value} with key {key}", value, httpContext.Request.Path);
        }
    }
}