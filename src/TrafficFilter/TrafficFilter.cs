using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrafficFilter.Configuration;
using TrafficFilter.Core;
using TrafficFilter.CoreFirewall;
using TrafficFilter.CoreRateLimiter;
using TrafficFilter.Extensions;

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
        private readonly TrafficFilterOptions _trafficFilterOptions;
        private readonly ILogger<TrafficFilter> _logger;
        private readonly IFirewall _firewall;
        private readonly IRateLimiter _rateLimiter;

        public TrafficFilter(
            IIpBlacklist ipBlacklist,
            IFirewall firewall,
            IRateLimiter rateLimiter,
            IOptions<TrafficFilterOptions> options,
            ILogger<TrafficFilter> logger)
        {
            _ipBlacklist = ipBlacklist ?? throw new ArgumentNullException(nameof(ipBlacklist));

            if (options == null) throw new ArgumentNullException(nameof(options));
            _trafficFilterOptions = options.Value ?? throw new ArgumentNullException(nameof(options.Value));

            _firewall = firewall ?? throw new ArgumentNullException(nameof(firewall), "Firewall cannot return null.");
            _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter), "RateLimiter cannot return null");

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

                if (_firewall.IsMatch(httpContext))
                {
                    _ipBlacklist.Add(ipAddress, _trafficFilterOptions.IPBlacklistTimeoutSeconds);                    
                    return false;
                }

                if (_rateLimiter.IsMatch(httpContext))
                {
                    _logger.LogInformation($"Rate limiting {ipAddress} - request rejected {httpContext.Request.Path.Value} {httpContext.Request.Method}");
                    _ipBlacklist.Add(ipAddress, _trafficFilterOptions.IPBlacklistTimeoutSeconds);
                    _logger.LogInformation($"Adding IP {ipAddress} to blacklist");
                    return false;
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