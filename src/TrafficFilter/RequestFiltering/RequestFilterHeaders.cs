using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TrafficFilter.Configuration;
using TrafficFilter.Matches;

namespace TrafficFilter.RequestFiltering
{
    public interface IRequestFilterHeaders : IRequestFilter
    {
        bool IsBadRequest(IHeaderDictionary headerDictionary);
    }

    public class RequestFilterHeaders : IRequestFilterHeaders
    {
        protected readonly RequestFilterHeadersOptions _options;

        private readonly IList<HeaderMatch> _matches;
        private readonly ILogger<RequestFilterHeaders> _logger;

        public RequestFilterHeaders(IOptions<RequestFilterHeadersOptions> options,
            IMatchesFactory matchesFactory,
            ILogger<RequestFilterHeaders> logger)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            if (matchesFactory == null) { throw new ArgumentNullException(nameof(matchesFactory)); }
            if (logger == null) { throw new ArgumentNullException(nameof(logger)); }

            _options = options.Value;

            _matches = _options.Matches != null
                ? _options.Matches.Select(m => new HeaderMatch(m.Header, matchesFactory.GetInstance(m.Type, m.Match))).ToList()
                : new List<HeaderMatch>();

            _logger = logger;
        }

        public bool IsEnabled => _options.IsEnabled;

        public bool IsBadRequest(IHeaderDictionary headerDictionary)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (headerDictionary == null)
            {
                _logger.LogWarning($"Bad Header Request detected - {nameof(headerDictionary)} is null");
                return true;
            }

            return IsMatch(headerDictionary);
        }

        private bool IsMatch(IHeaderDictionary headerDictionary)
        {
            foreach (var m in _matches)
            {
                if (!headerDictionary.ContainsKey(m.Header))
                {
                    _logger.LogWarning($"Bad Header Request detected - header '{m.Header}' was not found");
                    return true;
                }

                if (m.Match.IsMatch(headerDictionary[m.Header]))
                {
                    _logger.LogWarning($"Bad Header Request detected - matched header '{m.Header}', match '{m.Match.Match}'  value '{headerDictionary[m.Header]}'");
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
