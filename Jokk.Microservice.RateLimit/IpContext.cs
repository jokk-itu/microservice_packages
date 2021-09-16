using System;
using System.Text.Json;
using System.Threading.Tasks;
using RedLockNet.SERedis;
using StackExchange.Redis;

namespace Jokk.Microservice.RateLimit
{
    //IpAddress is a key in Redis
    //IpAddress is also a resource
    //Locks work on resources
    public class IpContext
    {
        private readonly RedLockFactory _lockFactory;
        private readonly RateLimitConfiguration _config;
        private readonly IDatabaseAsync _db;
        
        public IpContext(IConnectionMultiplexer conn, RedLockFactory lockFactory, RateLimitConfiguration config)
        {
            _lockFactory = lockFactory;
            _config = config;
            _db = conn.GetDatabase();
        }

        //Acquire a distributed RedisLock and then update
        public async Task<bool> UpdateRateAsync(string ipAddress)
        {
            var expires = TimeSpan.FromSeconds(3);
            var wait = TimeSpan.FromSeconds(5);
            var retry = TimeSpan.FromSeconds(1);

            var value = await _db.StringGetAsync(ipAddress);
            var rateLimit = new RateLimit(_config);
            
            if(value.IsNullOrEmpty)
                rateLimit = JsonSerializer.Deserialize<RateLimit>(value);
            
            if (rateLimit != null && rateLimit.IsLimitReached())
                return false;
            
            await using var redisLock = await _lockFactory.CreateLockAsync(ipAddress, expires, wait, retry);
            if (redisLock.IsAcquired)
            {
                rateLimit?.IncrementValues();
                return await _db.StringSetAsync(
                    ipAddress, JsonSerializer.Serialize(rateLimit));
            }

            return false;
        }
    }
}