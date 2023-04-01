using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;

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
        private readonly ILogger<TrafficFilter> _logger;

        public TrafficFilter(IIpBlacklist ipBlacklist,
            IRequestFiltersFactory requestFiltersFactory,
            IOptions<TrafficFilterOptions> options,
            ILogger<TrafficFilter> logger)
        {
            _ipBlacklist = ipBlacklist ?? throw new ArgumentNullException(nameof(ipBlacklist));
            _requestFiltersFactory = requestFiltersFactory ?? throw new ArgumentNullException(nameof(requestFiltersFactory));

            if (options == null) throw new ArgumentNullException(nameof(options));
            _trafficFilterOptions = options.Value ?? throw new ArgumentNullException(nameof(options.Value));

            _logger = logger;
        }

        public TrafficFilterOptions TrafficFilterOptions => _trafficFilterOptions;

        public bool IsAllowed(HttpContext httpContext)
        {
            try
            {
                var ipAddress = httpContext.GetIPAddress();

                _logger.LogInformation($"IP: {ipAddress} {httpContext.Request.Method} {httpContext.GetDisplayUrl()}");

                if (_ipBlacklist.IsInBlacklist(ipAddress))
                {
                    _logger.LogInformation($"Already blacklisted {ipAddress} - request rejected {httpContext.Request.Path.Value} {httpContext.Request.Method}");
                    return false;
                }

                foreach (var requestFilter in _requestFiltersFactory.RequestFilters)
                {
                    if (requestFilter.IsMatch(httpContext))
                    {
                        _ipBlacklist.Add(ipAddress, _trafficFilterOptions.IPBlacklistTimeoutSeconds);
                        _logger.LogInformation($"Adding IP {ipAddress} to blacklist");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return true;
        }
    }
}