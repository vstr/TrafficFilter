using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrafficFilter.Core;
using TrafficFilter.CoreFirewall;
using TrafficFilter.CoreRateLimiter.Configuration;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;

namespace TrafficFilter.CoreRateLimiter
{
    public interface IRateLimiter
    {
        bool IsMatch(HttpContext httpContext);
    }

    public class RateLimiter : IRateLimiter
    {
        private readonly RateLimiterOptions _options;
        private readonly Dictionary<IPAddress, RequestsQueue> _ipRequestsQueues = new Dictionary<IPAddress, RequestsQueue>();
        private static SpinLock _spinlock;
        private readonly TimeSpan _expiresIn;
        private readonly RulesContainer _rulesContainer;
        private readonly ILogger<RateLimiter> _logger;

        public RateLimiter(IOptions<RateLimiterOptions> options,
            IMatchesFactory matchesFactory,
            ILogger<RateLimiter> logger)
        {
            if (matchesFactory == null) { throw new ArgumentNullException(nameof(matchesFactory)); }

            _options = options.Value;

            _expiresIn = TimeSpan.FromSeconds(_options.WindowSeconds);

            if (_options.IsEnabled)
            {
                _options.Validate();

                _rulesContainer = new RulesContainer(matchesFactory, _options.WhitelistRules);
            }

            _logger = logger;
        }

        public bool IsMatch(HttpContext httpContext)
        {
            if (!_options.IsEnabled)
            {
                return false;
            }

            var whitelistMatchResult = _rulesContainer.IsMatch(httpContext);

            if (whitelistMatchResult != null && whitelistMatchResult.Count > 0)
            {
                return false; // If any whitelist rule matches, skip rate limiting
            }

            return IsFull(httpContext);
        }

        private bool IsFull(HttpContext httpContext)
        {
            var lockTaken = false;
            try
            {
                _spinlock.Enter(ref lockTaken);

                if (!_ipRequestsQueues.TryGetValue(httpContext.GetIPAddress(), out var ipRequestsQueue))
                {
                    ipRequestsQueue = new RequestsQueue(_options.RequestsLimit, _expiresIn);
                    _ipRequestsQueues[httpContext.GetIPAddress()] = ipRequestsQueue;
                }

                var isFull = ipRequestsQueue.IsFull();

                if (isFull)
                {
                    _logger.LogInformation($"RateLimiter detected for {httpContext.GetIPAddress()} path {httpContext.GetDisplayUrl()}");
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