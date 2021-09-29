using System;

namespace TrafficFilter.RateLimit
{
    public class RequestItem
    {
        public DateTime Created { get; set; }

        public int RequestHash { get; set; }
    }
}
