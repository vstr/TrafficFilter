using System.Collections.Generic;

namespace TrafficFilter.Configuration
{
    public class RequestFilterUrlOptions
    {
        public const string RequestFilterUrl = nameof(RequestFilterUrl);

        public bool IsEnabled { get; set; }
        public IList<MatchItemUrl> Matches { get; set; }
    }
}
