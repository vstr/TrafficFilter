using System.Collections.Generic;

namespace TrafficFilter.Configuration
{
    public class RequestFilterRateLimiterByPathOptions
    {
        public const string RateLimiterByPath = "RateLimiterByPath";

        public bool IsEnabled { get; set; }
        public int RateLimiterWindowSeconds { get; set; } = 1;
        public int RateLimiterRequestLimit { get; set; } = 7;
        public IList<MatchItemUrl> WhitelistUrls { get; set; }
    }
}