using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrafficFilter.Configuration;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;

namespace TrafficFilter.RequestFilters
{
    /// <summary>
    /// Request filter based on IP address with the widlcard match for some digits, e.g. 192.168.1.*  192.168.*.*
    /// </summary>
    public class RequestFilterIp : IRequestFilter
    {
        private readonly RequestFilterIpOptions _options;
        private readonly ILogger<RequestFilterIp> _logger;
        private readonly IList<IMatch> _matches;

        public RequestFilterIp(IOptions<RequestFilterIpOptions> options,
            IMatchesFactory matchesFactory,
            ILogger<RequestFilterIp> logger)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            if (matchesFactory == null) { throw new ArgumentNullException(nameof(matchesFactory)); }

            _options = options.Value ?? throw new ArgumentNullException(nameof(options.Value));

            _matches = _options.Matches.Select(m =>
                {
                    if (m.Match.Contains("*"))
                    {
                        return (IMatch)new MatchRegex(m.Match.Replace(".", "\\.").Replace("*", "\\d+"));
                    }
                    return (IMatch)new MatchExact(m.Match);
                }
            ).ToList();

            _logger = logger;
        }

        public bool IsEnabled => _options.IsEnabled;
        public int Order => 5;

        public bool IsMatch(HttpContext httpContext)
        {
            if (!IsEnabled)
            {
                return false;
            }

            var ipAddress = httpContext.GetIPAddress().ToString();

            if (string.IsNullOrEmpty(ipAddress))
            {
                _logger.LogInformation($"Bad IP Address Request detected - '{nameof(ipAddress)}' is null");
                return true;
            }

            foreach (var m in _matches)
            {
                if (m.IsMatch(ipAddress))
                {
                    _logger.LogInformation($"Blacklisted IP Address Request detected - '{ipAddress}' match '{m.Match}'");
                    return true;
                }
            }
            return false;
        }
    }
}