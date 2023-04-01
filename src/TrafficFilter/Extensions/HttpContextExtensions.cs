using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

using System.Net;

using TrafficFilter.Extensions;

namespace TrafficFilter.Extensions
{
    public static class HttpContextExtensions
    {
        private const string DisplayUrlKey = nameof(DisplayUrlKey);
        private const string IPAddressItemKey = nameof(IPAddressItemKey);

        public static string GetDisplayUrl(this HttpContext httpContext)
        {
            if (httpContext == null
                || httpContext.Request.Host == null
                || string.IsNullOrEmpty(httpContext.Request.Host.Value))
            {
                return null;
            }

            if (httpContext.Items.TryGetValue(DisplayUrlKey, out var displayUrl))
            {
                return (string)displayUrl;
            }

            displayUrl = httpContext.Request.GetDisplayUrl();

            if (displayUrl == null) { return null; }

            httpContext.Items[DisplayUrlKey] = displayUrl;
            return (string)displayUrl;
        }

        public static IPAddress GetIPAddress(this HttpContext httpContext)
        {
            if (httpContext == null) { return null; }

            if (httpContext.Items.TryGetValue(IPAddressItemKey, out var ipAddress))
            {
                return (IPAddress)ipAddress;
            }

            ipAddress = httpContext.Connection.RemoteIpAddress;

            if (!httpContext.Items.ContainsKey(IPAddressItemKey))
            {
                httpContext.Items[IPAddressItemKey] = ipAddress;
            }

            return (IPAddress)ipAddress;
        }
    }
}