using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jokk.Microservice.Prometheus.Constants;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Jokk.Microservice.Prometheus.HealthChecks
{
    internal class Neo4JHealthCheck : IHealthCheck
    {
        private readonly PrometheusConfiguration _configuration;
        private readonly HttpClient _httpClient;

        private readonly string _cluster;
        private readonly string _databaseAvailable;
        private readonly string _databaseHealthy;
        
        public Neo4JHealthCheck(PrometheusConfiguration configuration, IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient(ClientName.HealthCheck);
            _configuration = configuration;
            
            _databaseAvailable = $"/db/{_configuration.Neo4JDatabase}/cluster/available";
            _databaseHealthy = $"/db/{_configuration.Neo4JDatabase}/cluster/status";
            _cluster = "/dbms/cluster/status";
            
            SetupClient();
        }
        
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
            =>
                await IsClusterHealthy() && 
                await IsDatabaseAvailable() && 
                await IsDatabaseHealthy() ? 
                    HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        
        private async Task<bool> IsClusterHealthy()
        {
            var response = await _httpClient.GetAsync(_cluster);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> IsDatabaseAvailable()
        {
            var response = await _httpClient.GetAsync(_databaseAvailable);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> IsDatabaseHealthy()
        {
            var response = await _httpClient.GetAsync(_databaseHealthy);
            return response.IsSuccessStatusCode;
        }

        private void SetupClient()
        {
            _httpClient.BaseAddress = new Uri(_configuration.Neo4JConnectionString);
        }
    }
}