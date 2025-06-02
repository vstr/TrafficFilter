using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;

using TrafficFilter.Configuration;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;

namespace TrafficFilter.RequestFilters
{
    public class RequestFilterUrl : IRequestFilter
    {
        private readonly RequestFilterUrlOptions _options;
        private readonly IList<IMatch> _matches;
        private readonly ILogger<RequestFilterUrl> _logger;

        public RequestFilterUrl(IOptions<RequestFilterUrlOptions> options,
            IMatchesFactory matchesFactory,
            ILogger<RequestFilterUrl> logger)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            if (matchesFactory == null) { throw new ArgumentNullException(nameof(matchesFactory)); }

            _options = options.Value ?? throw new ArgumentNullException(nameof(options.Value));

            _matches = _options.Matches != null
                ? _options.Matches.Select(m => matchesFactory.GetInstance(m.Type, m.Match)).ToList()
                : new List<IMatch>();

            _logger = logger;
        }

        public bool IsEnabled => _options.IsEnabled;
        public int Order => 10;

        public bool IsMatch(HttpContext httpContext)
        {
            if (!IsEnabled)
            {
                return false;
            }

            var displayUrl = httpContext.GetDisplayUrl();

            if (string.IsNullOrEmpty(displayUrl))
            {
                _logger.LogInformation($"Bad Url Request detected - '{nameof(displayUrl)}' is null");
                return true;
            }

            foreach (var m in _matches)
            {
                if (m.IsMatch(displayUrl))
                {
                    _logger.LogInformation($"Bad Url Request detected - '{displayUrl}' match '{m.Match}'");
                    return true;
                }
            }

            return false;
        }
    }
}