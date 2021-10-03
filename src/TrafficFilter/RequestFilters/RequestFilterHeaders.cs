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
    public class RequestFilterHeaders : IRequestFilter
    {
        protected readonly RequestFilterHeadersOptions _options;
        private readonly IList<HeaderMatch> _headerMatches;

        public RequestFilterHeaders(IOptions<RequestFilterHeadersOptions> options,
            IMatchesFactory matchesFactory)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            if (matchesFactory == null) { throw new ArgumentNullException(nameof(matchesFactory)); }

            _options = options.Value;

            _headerMatches = _options.Matches != null
                ? _options.Matches.Select(m => new HeaderMatch(m.Header, matchesFactory.GetInstance(m.Type, m.Match))).ToList()
                : new List<HeaderMatch>();
        }

        public bool IsEnabled => _options.IsEnabled;
        public int Order => 2;

        public bool IsMatch(HttpContext httpContext)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (httpContext.Request.Headers == null)
            {
                httpContext.Log(LogLevel.Information, $"Bad Header Request detected - {nameof(httpContext.Request.Headers)} is null");
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
                    httpContext.Log(LogLevel.Information, $"Bad Header Request detected - header '{m.Header}' was not found");
                    return true;
                }

                if (m.Match.IsMatch(httpContext.Request.Headers[m.Header]))
                {
                    httpContext.Log(LogLevel.Information, $"Bad Header Request detected - matched header '{m.Header}', match '{m.Match.Match}'  value '{httpContext.Request.Headers[m.Header]}'");
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