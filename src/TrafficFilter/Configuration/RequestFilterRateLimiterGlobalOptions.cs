using System.Collections.Generic;

namespace TrafficFilter.Configuration
{
    public class RequestFilterRateLimiterGlobalOptions
    {
        public const string RateLimiterGlobal = "RateLimiterGlobal";

        public bool IsEnabled { get; set; }
        public int RateLimiterWindowSeconds { get; set; } = 1;
        public int RateLimiterRequestLimit { get; set; } = 7;
        public IList<MatchItemUrl> WhitelistUrls { get; set; }
    }
}