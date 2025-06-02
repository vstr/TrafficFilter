using System.Collections.Generic;

namespace TrafficFilter.Configuration
{
    public class RequestFilterIpOptions
    {
        public const string RequestFilterIp = nameof(RequestFilterIp);
        public bool IsEnabled { get; set; }
        public IList<MatchItemIp> Matches { get; set; }
    }
}