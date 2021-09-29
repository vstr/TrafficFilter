using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TrafficFilter.Configuration;
using TrafficFilter.Matches;

namespace TrafficFilter.RateLimit
{
    public interface IRateLimiter
    {
        bool IsEnabled { get; }

        bool IsLimitReached(string ipAddress, string request);
    }

    public class RateLimiter : IRateLimiter
    {
        private readonly RateLimiterOptions _options;
        private readonly Dictionary<string, RequestBuffer> _requests = new Dictionary<string, RequestBuffer>();
        private static SpinLock _spinlock;
        private readonly TimeSpan _expiresIn;
        private readonly ILogger<RateLimiter> _logger;
        private readonly IList<IMatch> _skipUrlMatches;

        public RateLimiter(IOptions<RateLimiterOptions> options,
            IMatchesFactory matchesFactory,
            ILogger<RateLimiter> logger)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            if (matchesFactory == null) { throw new ArgumentNullException(nameof(matchesFactory)); }
            if (logger == null) { throw new ArgumentNullException(nameof(logger)); }

            _options = options.Value;
            _expiresIn = TimeSpan.FromSeconds(_options.RateLimiterWindowSeconds);

            _skipUrlMatches = _options.SkipUrls != null
               ? _options.SkipUrls.Select(m => matchesFactory.GetInstance(m.Type, m.Match)).ToList()
               : new List<IMatch>();

            _logger = logger;
        }

        public bool IsEnabled => _options.IsEnabled;

        public bool IsLimitReached(string ipAddress, string request)
        {
            if (!IsEnabled)
            {
                return false;
            }

            foreach (var m in _skipUrlMatches)
            {
                if (m.IsMatch(request))
                {
                    return false;
                }
            }

            return IsFull(ipAddress, request);
        }

        private bool IsFull(string ipAddress, string request)
        {
            var lockTaken = false;
            try
            {
                _spinlock.Enter(ref lockTaken);

                if (!_requests.TryGetValue(ipAddress, out var list))
                {
                    list = new RequestBuffer(_options.RateLimiterRequestLimit, _expiresIn);
                    _requests[ipAddress] = list;
                }

                var isFull = list.IsFull(request);

                if (isFull)
                {
                    _logger.LogWarning($"Rate limit detected {ipAddress} {request}");
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
