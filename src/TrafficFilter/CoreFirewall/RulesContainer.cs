using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using TrafficFilter.Configuration;
using TrafficFilter.Matches;
using TrafficFilter.Rules;

namespace TrafficFilter.CoreFirewall
{
    public class RulesContainer
    {
        private readonly Dictionary<string, List<RuleBase>> _blockRules;

        public RulesContainer(
            IMatchesFactory matchesFactory,
            List<RuleOptions> ruleOptions)
        {
            if (ruleOptions != null)
            {
                _blockRules = ruleOptions.Select(r => RuleBase.BuildRule(matchesFactory, r))
                    .GroupBy(r => r.Group)
                    .ToDictionary(k => k.Key, v => v.ToList());
            }
        }

        public List<RuleBase> IsMatch(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (_blockRules == null || _blockRules.Count == 0)
            {
                return null; // No rules to match against
            }

            var matchedRules = new List<RuleBase>();
            foreach (var kvp in _blockRules)
            {
                if (kvp.Value.All(r => r.IsMatch(httpContext)))
                {
                    return kvp.Value; // If all rules in the group match, return the whole group
                }
            }

            return null; // If no rules matched, return null
        }
    }
}