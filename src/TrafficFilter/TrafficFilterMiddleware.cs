using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TrafficFilter.Configuration;
using TrafficFilter.RateLimit;
using TrafficFilter.RequestFiltering;

namespace TrafficFilter
{
    public class TrafficFilterMiddleware
    {
        private readonly RequestDelegate _next;

        public TrafficFilterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext,
            IIpBlacklist ipBlacklist,
            IRequestFilterUrl requestUrlFilter,
            IRequestFilterHeaders requestHeadersFilter,
            IRateLimiter rateLimiter,
            IOptions<TrafficFilterOptions> options,
            ILogger<TrafficFilterMiddleware> logger)
        {
            var ipAddress = httpContext.GetIPAddress();

            if (ipAddress == null || ipBlacklist.IsInBlacklist(ipAddress))
            {
                SetupResponse(httpContext.Response, options.Value.IPBlacklistTimeoutSeconds);
                logger.LogWarning($"Already blacklisted {ipAddress} - request rejected {httpContext.Request.Path.Value} {httpContext.Request.Method}");
                return;
            }

            string requestString;
            try
            {
                requestString = httpContext.Request.GetDisplayUrl();
                logger.LogInformation($"{ipAddress} {requestString} {httpContext.Request.Method}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                AddIpToBlacklist(httpContext, ipBlacklist, ipAddress, options.Value.IPBlacklistTimeoutSeconds, logger);
                return;
            }

            if (rateLimiter.IsLimitReached(ipAddress, httpContext.Request.Path)
                || requestUrlFilter.IsBadRequest(requestString)
                || requestHeadersFilter.IsBadRequest(httpContext.Request.Headers))
            {
                AddIpToBlacklist(httpContext, ipBlacklist, ipAddress, options.Value.IPBlacklistTimeoutSeconds, logger);
                return;
            }

            await _next.Invoke(httpContext);
        }

        private void AddIpToBlacklist(HttpContext httpContext, IIpBlacklist ipBlacklist, string ipAddress,
            int ipBlacklistTimeoutSeconds,
            ILogger<TrafficFilterMiddleware> logger)
        {
            ipBlacklist.Add(ipAddress, ipBlacklistTimeoutSeconds);
            SetupResponse(httpContext.Response, ipBlacklistTimeoutSeconds);
            logger.LogWarning($"Adding IP {ipAddress} to blacklist");
        }

        private static void SetupResponse(HttpResponse response, int blacklistTimeoutSeconds)
        {
            response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            response.Headers["Retry-After"] = blacklistTimeoutSeconds.ToString();
        }
    }
}