using System;
using System.Collections.Generic;
using TrafficFilter.Configuration;

namespace TrafficFilter.CoreRateLimiter.Configuration
{
    public class RateLimiterOptions
    {
        public const string SectionName = "RateLimiter";

        public bool IsEnabled { get; set; }
        public int WindowSeconds { get; set; } = 1;
        public int RequestsLimit { get; set; } = 7;
        public IList<RuleOptions> WhitelistRules { get; set; }

        public void Validate()
        {
            if (WindowSeconds <= 0)
            {
                throw new ArgumentException("WindowSeconds must be greater than zero.", nameof(WindowSeconds));
            }

            if (RequestsLimit <= 0)
            {
                throw new ArgumentException("RequestsLimit must be greater than zero.", nameof(RequestsLimit));
            }

            if (WhitelistRules != null)
            {
                foreach (var rule in WhitelistRules)
                {
                    rule.Validate();
                }
            }
        }
    }
}