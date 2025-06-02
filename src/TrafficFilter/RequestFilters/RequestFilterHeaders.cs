using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;

using TrafficFilter.Configuration;
using TrafficFilter.Matches;

namespace TrafficFilter.RequestFilters
{
    public class RequestFilterHeaders : IRequestFilter
    {
        protected readonly RequestFilterHeadersOptions _options;
        private readonly IList<HeaderMatch> _headerMatches;
        private readonly ILogger<RequestFilterHeaders> _logger;

        public RequestFilterHeaders(IOptions<RequestFilterHeadersOptions> options,
            IMatchesFactory matchesFactory,
            ILogger<RequestFilterHeaders> logger)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            if (matchesFactory == null) { throw new ArgumentNullException(nameof(matchesFactory)); }

            _options = options.Value;

            _headerMatches = _options.Matches != null
                ? _options.Matches.Select(m => new HeaderMatch(m.Header, matchesFactory.GetInstance(m.Type, m.Match))).ToList()
                : new List<HeaderMatch>();

            _logger = logger;
        }

        public bool IsEnabled => _options.IsEnabled;
        public int Order => 20;

        public bool IsMatch(HttpContext httpContext)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (httpContext.Request.Headers == null)
            {
                _logger.LogInformation($"Bad Header Request detected - {nameof(httpContext.Request.Headers)} is null");
                return true;
            }

            return IsHeaderMatch(httpContext);
        }

        private bool IsHeaderMatch(HttpContext httpContext)
        {
            foreach (var m in _headerMatches)
            {
                if (!httpContext.Request.Headers.ContainsKey(m.Header))
                {
                    _logger.LogInformation($"Bad Header Request detected - header '{m.Header}' was not found");
                    return true;
                }

                if (m.Match.IsMatch(httpContext.Request.Headers[m.Header]))
                {
                    _logger.LogInformation($"Bad Header Request detected - matched header '{m.Header}', match '{m.Match.Match}'  value '{httpContext.Request.Headers[m.Header]}'");
                    return true;
                }
            }

            return false;
        }

        private class HeaderMatch
        {
            public HeaderMatch(string header, IMatch match)
            {
                Header = header;
                Match = match;
            }

            public string Header { get; }
            public IMatch Match { get; }
        }
    }
}