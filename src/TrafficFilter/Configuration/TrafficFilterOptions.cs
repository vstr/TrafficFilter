using System;

namespace TrafficFilter.Configuration
{
    public class TrafficFilterOptions
    {
        public const string TrafficFilter = nameof(TrafficFilter);
        public int IPBlacklistTimeoutSeconds { get; set; } = (int)TimeSpan.FromMinutes(15).TotalSeconds;
    }
}
