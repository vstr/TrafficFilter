using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TrafficFilter.Configuration;
using TrafficFilter.Matches;

namespace TrafficFilter.RequestFiltering
{
    public interface IRequestFilterUrl : IRequestFilter
    {
        bool IsBadRequest(string requestUrl);
    }

    public class RequestFilterUrl : IRequestFilterUrl
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
            if (logger == null) { throw new ArgumentNullException(nameof(logger)); }

            _options = options.Value;

            _matches = _options.Matches != null
                ? _options.Matches.Select(m => matchesFactory.GetInstance(m.Type, m.Match)).ToList()
                : new List<IMatch>();

            _logger = logger;
        }

        public bool IsEnabled => _options.IsEnabled;

        public bool IsBadRequest(string requestUrl)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (requestUrl == null)
            {
                _logger.LogWarning($"Bad Url Request detected - '{nameof(requestUrl)}' is null");
                return true;
            }

            foreach (var m in _matches)
            {
                if (m.IsMatch(requestUrl))
                {
                    _logger.LogWarning($"Bad Url Request detected - '{requestUrl}' match '{m.Match}'");
                    return true;
                }
            }

            return false;
        }
    }
}
