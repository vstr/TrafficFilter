using System;
using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

using TrafficFilter.Extensions;

namespace TrafficFilter.Extensions
{
    public static class HttpContextExtensions
    {
        private const string DisplayUrlKey = nameof(DisplayUrlKey);
        private const string IPAddressItemKey = nameof(IPAddressItemKey);

        private static ILogger GetLogger(this HttpContext context)
        {
            return (ILogger)context.RequestServices?.GetService(typeof(ILogger<TrafficFilter>));
        }

        internal static void Log(this HttpContext context, LogLevel logLevel, string message)
        {
            var logger = context.GetLogger();

            if (logger == null || !logger.IsEnabled(logLevel))
            {
                return;
            }

            logger.Log(logLevel, message);
        }

        public static string GetDisplayUrl(this HttpContext httpContext)
        {
            if (httpContext == null) { return null; }

            if (!httpContext.Items.TryGetValue(DisplayUrlKey, out var displayUrl))
            {
                try
                {
                    httpContext.Items[DisplayUrlKey] = displayUrl = httpContext.Request.GetDisplayUrl();
                    return (string)displayUrl;
                }
                catch (Exception ex)
                {
                    httpContext.Log(LogLevel.Error, ex.ToString());
                }
            }

            return (string)displayUrl;
        }

        public static IPAddress GetIPAddress(this HttpContext httpContext)
        {
            if (httpContext == null) { return null; }

            if (!httpContext.Items.TryGetValue(IPAddressItemKey, out var ipAddress))
            {
                ipAddress = httpContext.Connection.RemoteIpAddress;

                httpContext.Log(LogLevel.Information, $"IP: {ipAddress} {httpContext.Request.Method} {httpContext.GetDisplayUrl()}");

                if (!httpContext.Items.ContainsKey(IPAddressItemKey))
                {
                    httpContext.Items[IPAddressItemKey] = ipAddress;
                }
            }

            return (IPAddress)ipAddress;
        }
    }
}