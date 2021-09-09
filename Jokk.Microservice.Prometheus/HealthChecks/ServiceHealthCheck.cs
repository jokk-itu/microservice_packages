using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jokk.Microservice.Prometheus.Constants;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Jokk.Microservice.Prometheus.HealthChecks
{
    public class ServiceHealthCheck : IHealthCheck
    {
        private readonly IDictionary<string, string> _uris;
        private readonly HttpClient _httpClient;
        
        public ServiceHealthCheck(IHttpClientFactory factory, PrometheusConfiguration configuration)
        {
            _uris = configuration.Services;
            _httpClient = factory.CreateClient(ClientName.HealthCheck);
        }
        
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var unhealthyServices = new Dictionary<string, object>();
            foreach (var (service, url) in _uris)
            {
                var response = await _httpClient.GetAsync($"{url}{HealthCheckEndpoint.Endpoint}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                    unhealthyServices.TryAdd(service,
                        $"Service: {service}, Uri: {url}, StatusCode: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            }
            var description = unhealthyServices.Any()
                ? unhealthyServices.Select(pair => $"{pair.Value}\n").ToString()
                : "All services are healthy";
            return unhealthyServices.Any()
                ? HealthCheckResult.Unhealthy(description) 
                : HealthCheckResult.Healthy();
        }
    }
}