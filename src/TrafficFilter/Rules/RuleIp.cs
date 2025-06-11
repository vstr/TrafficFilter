using Microsoft.AspNetCore.Http;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;

namespace TrafficFilter.Rules
{
    public class RuleIp : RuleBase
    {
        public RuleIp(IMatch match)
            : base(match)
        {
        }

        protected override string GetRequestPartValue(HttpContext httpContext)
        {
            var ipAddress = httpContext.GetIPAddress().ToString();
            return ipAddress ?? string.Empty;
        }
    }
}