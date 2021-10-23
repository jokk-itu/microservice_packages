using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jokk.Microservice.HealthCheck.Constants;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Jokk.Microservice.HealthCheck.HealthChecks
{
    internal class ServiceHealthCheck : IHealthCheck
    {
        private readonly string _service;
        private readonly HttpClient _httpClient;

        public ServiceHealthCheck(IHttpClientFactory factory, string service, Uri uri)
        {
            _service = service;
            _httpClient = factory.CreateClient(ClientName.HealthCheck);
            _httpClient.BaseAddress = uri;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(HealthCheckEndpoint.Endpoint, cancellationToken);
            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy($"Service {_service} is healthy")
                : HealthCheckResult.Unhealthy(
                    $"Service {_service} is unhealthy, Status {response.StatusCode}, Reason {response.ReasonPhrase}");
        }
    }
}