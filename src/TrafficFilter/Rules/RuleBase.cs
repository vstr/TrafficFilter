using System;
using Microsoft.AspNetCore.Http;
using TrafficFilter.Configuration;
using TrafficFilter.Matches;

namespace TrafficFilter.Rules
{
    public abstract class RuleBase
    {
        private readonly IMatch _match;

        public RuleBase(IMatch match)
        {
            _match = match ?? throw new ArgumentNullException(nameof(match), "Match cannot be null.");
        }

        public string MatchType => _match.GetType().Name.Replace("Match", string.Empty);
        public string MatchValue => _match.Match;
        public string Group => _match.Group;

        public bool IsMatch(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var requestPartValue = GetRequestPartValue(httpContext);

            var isMatch = _match.IsMatch(requestPartValue);

            return isMatch;
        }

        protected abstract string GetRequestPartValue(HttpContext httpContext);

        public static RuleBase BuildRule(
            IMatchesFactory matchesFactory,
            RuleOptions ruleOptions)
        {
            var match = matchesFactory.GetInstance(ruleOptions);

            var requestPart = ruleOptions.RequestPart.Split(":");

            switch (requestPart[0].ToLowerInvariant())
            {
                case "header":
                    if (requestPart.Length < 2)
                    {
                        throw new ArgumentException("Header name is required for header-based firewall rule.", nameof(ruleOptions));
                    }
                    return new RuleHeader(match, requestPart[1]);

                case "ip":
                    return new RuleIp(match);

                case "url":
                    return new RuleUrl(match);

                default:
                    throw new NotSupportedException($"Request part '{requestPart[0]}' is not supported.");
            }
        }
    }
}