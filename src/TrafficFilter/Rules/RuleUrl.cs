using Microsoft.AspNetCore.Http;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;

namespace TrafficFilter.Rules
{
    public class RuleUrl : RuleBase
    {
        public RuleUrl(IMatch match)
            : base(match)
        {
        }

        protected override string GetRequestPartValue(HttpContext httpContext)
        {
            var displayUrl = httpContext.GetDisplayUrl();

            return displayUrl;
        }
    }
}