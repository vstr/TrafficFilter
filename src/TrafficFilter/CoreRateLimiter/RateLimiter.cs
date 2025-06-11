using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrafficFilter.Core;
using TrafficFilter.CoreRateLimiter.Configuration;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;
using TrafficFilter.Rules;

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
        private readonly IList<RuleBase> _whitelistRules;
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
                _whitelistRules = _options.WhitelistRules != null
                   ? _options.WhitelistRules.Select(r => RuleBase.BuildRule(matchesFactory, r)).ToList()
                   : new List<RuleBase>();
            }

            _logger = logger;
        }

        public bool IsMatch(HttpContext httpContext)
        {
            if (!_options.IsEnabled)
            {
                return false;
            }

            foreach (var rule in _whitelistRules)
            {
                if (rule.IsMatch(httpContext))
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