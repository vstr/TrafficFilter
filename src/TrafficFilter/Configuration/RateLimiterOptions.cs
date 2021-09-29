using System.Collections.Generic;

namespace TrafficFilter.Configuration
{
    public class RateLimiterOptions
    {
        public const string RateLimiter = "RateLimiter";
        public bool IsEnabled { get; set; }
        public int RateLimiterWindowSeconds { get; set; } = 1;
        public int RateLimiterRequestLimit { get; set; } = 7;
        public IList<MatchItemUrl> SkipUrls { get; set; }
    }
}
