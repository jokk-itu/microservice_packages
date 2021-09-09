using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jokk.Microservice.Prometheus.Constants;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Jokk.Microservice.Prometheus.HealthChecks
{
    public class ServiceHealthCheck : IHealthCheck
    {
        private readonly string _service;
        private readonly string _uri;
        private readonly HttpClient _httpClient;

        public ServiceHealthCheck(IHttpClientFactory factory, string service, string uri)
        {
            _service = service;
            _uri = uri;
            _httpClient = factory.CreateClient(ClientName.HealthCheck);
            _httpClient.BaseAddress = new Uri(_uri);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _httpClient.GetAsync(HealthCheckEndpoint.Endpoint, cancellationToken);
            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy($"Service {_service} is healthy")
                : HealthCheckResult.Unhealthy(
                    $"Service {_service} is unhealthy, Status {response.StatusCode}, Reason {response.ReasonPhrase}");
        }
    }
}