using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TrafficFilter.Configuration;
using TrafficFilter.Core;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;

namespace TrafficFilter.RequestFilters
{
    public class RequestFilterRateLimiter : IRequestFilter
    {
        private readonly RequestFilterRateLimiterOptions _options;
        private readonly Dictionary<IPAddress, RequestBuffer> _requests = new Dictionary<IPAddress, RequestBuffer>();
        private static SpinLock _spinlock;
        private readonly TimeSpan _expiresIn;
        private readonly IList<IMatch> _whitelistUrlMatches;

        public RequestFilterRateLimiter(IOptions<RequestFilterRateLimiterOptions> options,
            IMatchesFactory matchesFactory)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            if (matchesFactory == null) { throw new ArgumentNullException(nameof(matchesFactory)); }

            _options = options.Value ?? throw new ArgumentNullException(nameof(options.Value));
            _expiresIn = TimeSpan.FromSeconds(_options.RateLimiterWindowSeconds);

            _whitelistUrlMatches = _options.WhitelistUrls != null
               ? _options.WhitelistUrls.Select(m => matchesFactory.GetInstance(m.Type, m.Match)).ToList()
               : new List<IMatch>();
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

                if (!_requests.TryGetValue(httpContext.GetIPAddress(), out var list))
                {
                    list = new RequestBuffer(_options.RateLimiterRequestLimit, _expiresIn);
                    _requests[httpContext.GetIPAddress()] = list;
                }

                var isFull = list.IsFull(httpContext.Request.Path);

                if (isFull)
                {
                    httpContext.Log(LogLevel.Information, $"Rate limit detected for {httpContext.GetIPAddress()} path {httpContext.Request.Path}");
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