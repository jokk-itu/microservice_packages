using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Jokk.Microservice.RateLimit.Extensions
{
    internal static class HttpContextExtensions
    {
        public static string GetIpAddress(this HttpContext httpContext)
        {
            var ip = httpContext.Request.Headers["HTTP_X_FORWARDED_FOR"].ToString();

            if (string.IsNullOrEmpty(ip))
            {
                ip = httpContext.Request.Headers["REMOTE_ADDR"];
            }
            else
            {
                ip = ip.Split(',')
                    .Last()
                    .Trim();
            }

            return ip;
        }
    }
}