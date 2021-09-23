using Microsoft.Extensions.Caching.Distributed;

namespace Jokk.Microservice.Cache.Distributed
{
    public class DistributedCacheContext
    {
        private readonly IDistributedCache _cacheStore;

        public DistributedCacheContext(IDistributedCache cacheStore)
        {
            _cacheStore = cacheStore;
        }
        
        //GetOrAdd
    }
}