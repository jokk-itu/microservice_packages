using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jokk.Microservice.Prometheus.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Jokk.Microservice.Prometheus.HealthChecks
{
    public class ServiceHealthCheck : IHealthCheck
    {
        private readonly IConfigurationSection _uris;
        private readonly HttpClient _httpClient;
        
        public ServiceHealthCheck(IHttpClientFactory factory, IConfigurationSection uris)
        {
            _uris = uris;
            _httpClient = factory.CreateClient();
        }
        
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var unhealthyServices = new Dictionary<string, object>();
            foreach (var service in _uris.GetChildren())
            {
                var response = await _httpClient.GetAsync($"{service.Value}{HealthCheckEndpoint.Endpoint}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                    unhealthyServices.TryAdd(service.Key, 
                        $"Service: {service.Key}, Uri: {service.Value}, StatusCode: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            }

            return unhealthyServices.Any() ? HealthCheckResult.Unhealthy(data: new ReadOnlyDictionary<string, object>(unhealthyServices)) : HealthCheckResult.Healthy();
        }
    }
}