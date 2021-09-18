using System.Collections.Concurrent;

namespace Jokk.Microservice.RateLimit.Concealed
{
    internal class ConcealedIpContext
    {
        private readonly RateLimitConfiguration _config;
        private readonly ConcurrentDictionary<string, RateLimit> _rateLimits;

        public ConcealedIpContext(RateLimitConfiguration config)
        {
            _config = config;
            _rateLimits = new ConcurrentDictionary<string, RateLimit>();
        }

        public bool UpdateRate(string ipAddress)
        {
            var rateLimit = _rateLimits.GetOrAdd(ipAddress, new RateLimit(_config));

            if (rateLimit.IsLimitReached())
                return false;

            var comparisonValue = rateLimit;
            rateLimit.IncrementValues();
            if (_rateLimits.TryUpdate(ipAddress, rateLimit, comparisonValue))
                throw new UpdateConcurrencyException($"{ipAddress} with {rateLimit} cannot be updated");
            
            return true;
        }
    }
}