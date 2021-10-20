using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Jokk.Microservice.Prometheus.HealthChecks
{
    internal class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _conn;
        private readonly PrometheusConfiguration _config;

        public RedisHealthCheck(IConnectionMultiplexer conn, PrometheusConfiguration config)
        {
            _conn = conn;
            _config = config;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var db = _conn.GetDatabase();
            try
            {
                var pong = await db.PingAsync();
                return pong.Milliseconds > 1000 
                    ? HealthCheckResult.Degraded() 
                    : HealthCheckResult.Healthy();
            }
            catch (Exception)
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}