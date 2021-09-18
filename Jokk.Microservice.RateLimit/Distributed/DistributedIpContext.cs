using System;
using System.Text.Json;
using System.Threading.Tasks;
using RedLockNet.SERedis;
using StackExchange.Redis;

namespace Jokk.Microservice.RateLimit.Distributed
{
    internal class DistributedIpContext
    {
        private readonly RedLockFactory _lockFactory;
        private readonly RateLimitConfiguration _config;
        private readonly IDatabaseAsync _db;
        
        public DistributedIpContext(IConnectionMultiplexer conn, RedLockFactory lockFactory, RateLimitConfiguration config)
        {
            _lockFactory = lockFactory;
            _config = config;
            _db = conn.GetDatabase();
        }

        //Acquire a distributed RedisLock and then update
        public async Task<bool> TryUpdateRateAsync(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentException("IPAddress must be given", nameof(ipAddress));
            
            var expires = TimeSpan.FromSeconds(3);
            var wait = TimeSpan.FromSeconds(5);
            var retry = TimeSpan.FromSeconds(1);

            var (isLimitReached, rateLimit) = await IsLimitReachedAsync(ipAddress);
            
            await using var redisLock = await _lockFactory.CreateLockAsync(ipAddress, expires, wait, retry);
            if (redisLock.IsAcquired && !isLimitReached)
            {
                rateLimit.IncrementValues();
                return await _db.StringSetAsync(
                    ipAddress, JsonSerializer.Serialize(rateLimit));
            }

            return false;
        }

        private async Task<(bool, RateLimit)> IsLimitReachedAsync(string ipAddress)
        {
            var value = await _db.StringGetAsync(ipAddress);
            var rateLimit = new RateLimit(_config);
            
            if(value.IsNullOrEmpty)
                rateLimit = JsonSerializer.Deserialize<RateLimit>(value);

            return (rateLimit != null && rateLimit.IsLimitReached(), rateLimit);
        }
    }
}