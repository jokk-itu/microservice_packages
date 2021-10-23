using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Neo4j.Driver;

namespace Jokk.Microservice.HealthCheck.HealthChecks
{
    internal class Neo4JHealthCheck : IHealthCheck
    {
        private readonly IDriver _driver;

        public Neo4JHealthCheck(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
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
            catch (Neo4jException)
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}