using Microsoft.Extensions.Caching.Memory;

namespace Jokk.Microservice.Cache.Memory
{
    public class MemoryCacheContext
    {
        private readonly IMemoryCache _cacheStore;

        public MemoryCacheContext(IMemoryCache cacheStore)
        {
            _cacheStore = cacheStore;
        }
        
        //GetOrAdd
    }
}