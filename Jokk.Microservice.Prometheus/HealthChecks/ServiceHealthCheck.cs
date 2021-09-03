using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly string[] _uris;
        private readonly HttpClient _httpClient;
        
        public ServiceHealthCheck(IHttpClientFactory factory, string[] uris)
        {
            _uris = uris;
            _httpClient = factory.CreateClient();
        }
        
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var unhealthyServices = new ReadOnlyDictionary<string,string>(new Dictionary<string, string>());
            foreach (var uri in _uris)
            {
                var response = await _httpClient.GetAsync($"{uri}{HealthCheckEndpoint.Endpoint}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                    unhealthyServices.TryAdd(uri, $"StatusCode: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            }

            return unhealthyServices.Any() ? HealthCheckResult.Unhealthy() : HealthCheckResult.Healthy();
        }
    }
}