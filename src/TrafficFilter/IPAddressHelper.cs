using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using TrafficFilter.Configuration;

namespace TrafficFilter
{
    public static class IPAddressHelper
    {
        public const string IPAddressItemKey = nameof(IPAddressItemKey);

        public static string GetIPAddress(this HttpContext httpContext, TrafficFilterOptions options, ILogger<TrafficFilterMiddleware> logger)
        {
            if (httpContext == null)
            {
                return null;
            }

            string remoteIpAddress = null;

            if (options.IsBehindReverseProxy)
            {
                remoteIpAddress = GetIPAddressFromHeaders(httpContext, logger);
            }

            if (string.IsNullOrEmpty(remoteIpAddress))
            {
                remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
                logger.LogDebug($"RemoteIpAddress: {remoteIpAddress}");
            }

            if (!httpContext.Items.ContainsKey(IPAddressItemKey))
            {
                httpContext.Items[IPAddressItemKey] = remoteIpAddress;
            }

            return remoteIpAddress;
        }

        public static string GetIPAddress(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return null;
            }

            if (httpContext.Items.TryGetValue(IPAddressItemKey, out var ipAddress))
            {
                return (string)ipAddress;
            }

            return null;
        }

        private static string GetIPAddressFromHeaders(HttpContext httpContext, ILogger<TrafficFilterMiddleware> logger)
        {
            //Check if we are behind CloudFlare
            string ipAddress = httpContext.Request.Headers["CF-Connecting-IP"];
            if (!string.IsNullOrEmpty(ipAddress))
            {
                logger.LogDebug($"CF-Connecting-IP: {ipAddress}");
                return ipAddress;
            }

            ipAddress = httpContext.Request.Headers["X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(ipAddress))
            {
                logger.LogDebug($"X_FORWARDED_FOR: {ipAddress}");
                ipAddress = ipAddress.Split(',').LastOrDefault().Trim();
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    return ipAddress;
                }
            }

            return null;
        }
    }
}