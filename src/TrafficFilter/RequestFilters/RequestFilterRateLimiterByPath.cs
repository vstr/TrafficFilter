using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

using TrafficFilter.Configuration;
using TrafficFilter.Core;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;

namespace TrafficFilter.RequestFilters
{
    public class RequestFilterRateLimiterByPath : IRequestFilter
    {
        private readonly RequestFilterRateLimiterByPathOptions _options;
        private readonly Dictionary<IPAddress, RequestBufferByPath> _requests = new Dictionary<IPAddress, RequestBufferByPath>();
        private static SpinLock _spinlock;
        private readonly TimeSpan _expiresIn;
        private readonly IList<IMatch> _whitelistUrlMatches;
        private readonly ILogger<RequestFilterRateLimiterByPath> _logger;

        public RequestFilterRateLimiterByPath(IOptions<RequestFilterRateLimiterByPathOptions> options,
            IMatchesFactory matchesFactory,
            ILogger<RequestFilterRateLimiterByPath> logger)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            if (matchesFactory == null) { throw new ArgumentNullException(nameof(matchesFactory)); }

            _options = options.Value ?? throw new ArgumentNullException(nameof(options.Value));
            _expiresIn = TimeSpan.FromSeconds(_options.RateLimiterWindowSeconds);

            _whitelistUrlMatches = _options.WhitelistUrls != null
               ? _options.WhitelistUrls.Select(m => matchesFactory.GetInstance(m.Type, m.Match)).ToList()
               : new List<IMatch>();
            _logger = logger;
        }

        public bool IsEnabled => _options.IsEnabled;
        public int Order => 3;

        public bool IsMatch(HttpContext httpContext)
        {
            if (!IsEnabled)
            {
                return false;
            }

            foreach (var m in _whitelistUrlMatches)
            {
                if (m.IsMatch(httpContext.GetDisplayUrl()))
                {
                    return false;
                }
            }

            return IsFull(httpContext);
        }

        private bool IsFull(HttpContext httpContext)
        {
            var lockTaken = false;
            try
            {
                _spinlock.Enter(ref lockTaken);

                if (!_requests.TryGetValue(httpContext.GetIPAddress(), out var requestPathBuffer))
                {
                    requestPathBuffer = new RequestBufferByPath(_options.RateLimiterRequestLimit, _expiresIn);
                    _requests[httpContext.GetIPAddress()] = requestPathBuffer;
                }

                var isFull = requestPathBuffer.IsFull(httpContext.Request.Path);

                if (isFull)
                {
                    _logger.LogInformation($"Rate limit ByPath detected for {httpContext.GetIPAddress()} path {httpContext.Request.Path}");
                }

                return isFull;
            }
            finally
            {
                if (lockTaken) _spinlock.Exit(false);
            }
        }
    }
}