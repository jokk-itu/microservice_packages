using System;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Jokk.Microservice.Polly
{
    public static class Setup
    {
        public static IHttpClientBuilder AddMicroservicePolicies(this IHttpClientBuilder client)
        {
            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2));

            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(1, TimeSpan.FromSeconds(10));
            
            //TODO Read up on Bulkhead isolation
            Policy.Bulkhead(20, 4);
            
            //TODO Caching policy
            
            //TODO fallback policy (return something else)
            
            client.AddPolicyHandler(retryPolicy).AddPolicyHandler(circuitBreakerPolicy);

            return client;
        }
    }
}