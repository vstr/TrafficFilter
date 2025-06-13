using System;
using System.Collections.Generic;
using System.Linq;
using TrafficFilter.Configuration;

namespace TrafficFilter.Matches
{
    public interface IMatchesFactory
    {
        IMatch GetInstance(RuleOptions ruleOptions);
    }

    public class MatchesFactory : IMatchesFactory
    {
        private Dictionary<string, Type> _matchTypes;

        public IMatch GetInstance(RuleOptions ruleOptions)
        {
            return Activator.CreateInstance(MatchTypes[ruleOptions.MatchType], ruleOptions.Match, ruleOptions.Group) as IMatch;
        }

        private Dictionary<string, Type> MatchTypes => _matchTypes ??= InitMatchTypes();

        private Dictionary<string, Type> InitMatchTypes()
        {
            return typeof(IMatch).Assembly
                 .GetTypes()
                 .Where(t => !t.IsAbstract)
                 .Where(t => typeof(IMatch).IsAssignableFrom(t))
                 .ToDictionary(t => t.Name.Replace("Match", string.Empty), t => t);
        }
    }
}