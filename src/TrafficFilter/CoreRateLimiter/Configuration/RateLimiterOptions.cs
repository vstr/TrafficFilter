using System.Collections.Generic;
using TrafficFilter.Configuration;

namespace TrafficFilter.CoreRateLimiter.Configuration
{
    public class RateLimiterOptions
    {
        public const string RateLimiter = nameof(RateLimiter);

        public bool IsEnabled { get; set; }
        public int WindowSeconds { get; set; } = 1;
        public int RequestsLimit { get; set; } = 7;
        public IList<RuleOptions> WhitelistRules { get; set; }
    }
}