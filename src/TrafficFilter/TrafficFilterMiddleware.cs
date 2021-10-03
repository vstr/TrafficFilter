using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace TrafficFilter
{


    public class TrafficFilterMiddleware
    {
        private readonly RequestDelegate _next;

        public TrafficFilterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext httpContext, ITrafficFilter trafficFilter)
        {
            return trafficFilter.IsAllowed(httpContext)
                        ? _next.Invoke(httpContext)
                        : BlockAccess(httpContext, trafficFilter.TrafficFilterOptions.IPBlacklistTimeoutSeconds);
        }

        private Task BlockAccess(HttpContext httpContext, int blacklistTimeoutSeconds)
        {
            httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            httpContext.Response.Headers["Retry-After"] = blacklistTimeoutSeconds.ToString();
            return Task.CompletedTask;
        }
    }
}