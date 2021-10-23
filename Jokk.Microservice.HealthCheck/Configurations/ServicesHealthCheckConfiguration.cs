using System.Collections.Generic;

namespace Jokk.Microservice.HealthCheck.Configurations
{
    public class ServicesHealthCheckConfiguration
    {
        public IDictionary<string, string> Services { get; init; }
    }
}