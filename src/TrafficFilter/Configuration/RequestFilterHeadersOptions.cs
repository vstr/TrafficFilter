using System.Collections.Generic;

namespace TrafficFilter.Configuration
{
    public class RequestFilterHeadersOptions
    {
        public const string RequestFilterHeaders = nameof(RequestFilterHeaders);

        public bool IsEnabled { get; set; }
        public IList<MatchItemHeader> Matches { get; set; }
    }
}
