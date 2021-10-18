using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Jokk.Microservice.Cache
{
    public interface ICacheStore
    {
        Task<T> GetValueAsync<T>(HttpContext httpContext, CancellationToken cancellationToken = default);

        Task AddValue(HttpContext httpContext, object value, CancellationToken cancellationToken = default);
    }
}