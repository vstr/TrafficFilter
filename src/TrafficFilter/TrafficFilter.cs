using System;
using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TrafficFilter.Configuration;
using TrafficFilter.Core;
using TrafficFilter.Extensions;
using TrafficFilter.RequestFilters;

namespace TrafficFilter
{
    public interface ITrafficFilter
    {
        bool IsAllowed(HttpContext httpContext);

        TrafficFilterOptions TrafficFilterOptions { get; }
    }

    public class TrafficFilter : ITrafficFilter
    {
        private readonly IIpBlacklist _ipBlacklist;
        private readonly IRequestFiltersFactory _requestFiltersFactory;
        private readonly TrafficFilterOptions _trafficFilterOptions;

        public TrafficFilter(IIpBlacklist ipBlacklist, IRequestFiltersFactory requestFiltersFactory, IOptions<TrafficFilterOptions> options)
        {
            _ipBlacklist = ipBlacklist ?? throw new ArgumentNullException(nameof(ipBlacklist));
            _requestFiltersFactory = requestFiltersFactory ?? throw new ArgumentNullException(nameof(requestFiltersFactory));
            if (options == null) throw new ArgumentNullException(nameof(options));
            _trafficFilterOptions = options.Value ?? throw new ArgumentNullException(nameof(options.Value));
        }

        public TrafficFilterOptions TrafficFilterOptions => _trafficFilterOptions;

        public bool IsAllowed(HttpContext httpContext)
        {
            var ipAddress = httpContext.GetIPAddress();

            if (_ipBlacklist.IsInBlacklist(ipAddress))
            {
                httpContext.Log(LogLevel.Information, $"Already blacklisted {ipAddress} - request rejected {httpContext.Request.Path.Value} {httpContext.Request.Method}");
                return false;
            }

            foreach (var requestFilter in _requestFiltersFactory.RequestFilters)
            {
                if (requestFilter.IsMatch(httpContext))
                {
                    _ipBlacklist.Add(ipAddress, _trafficFilterOptions.IPBlacklistTimeoutSeconds);
                    LogRequest(httpContext, ipAddress);
                    return false;
                }
            }

            return true;
        }

        private void LogRequest(HttpContext httpContext, IPAddress ipAddress)
        {
            httpContext.Log(LogLevel.Information, $"Adding IP {ipAddress} to blacklist");
            httpContext.Log(LogLevel.Information, $" Request|Url|{httpContext.GetDisplayUrl()}");
            if (httpContext.Request.Headers != null)
            {
                foreach (var h in httpContext.Request.Headers)
                {
                    httpContext.Log(LogLevel.Information, $" Header|{h.Key}|{h.Value}");
                }
            }
        }
    }
}