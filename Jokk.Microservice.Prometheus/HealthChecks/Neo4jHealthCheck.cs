using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jokk.Microservice.Prometheus.Constants;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Neo4j.Driver;

namespace Jokk.Microservice.Prometheus.HealthChecks
{
    internal class Neo4JHealthCheck : IHealthCheck
    {
        private readonly PrometheusConfiguration _configuration;
        private readonly IDriver _driver;

        public Neo4JHealthCheck(PrometheusConfiguration configuration, IDriver driver)
        {
            _configuration = configuration;
            _driver = driver;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var session = _driver.AsyncSession();
                var response = await session.ReadTransactionAsync(async transaction =>
                {
                    const string cypher =
                        @"MATCH (n) RETURN count(*) as count";
                    var result = await transaction.RunAsync(cypher);
                    return await result.PeekAsync();
                });
                return response is not null ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
            }
            catch (Neo4jException exception)
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}