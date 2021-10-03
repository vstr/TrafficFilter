using System;
using System.Collections.Generic;
using System.Linq;

namespace TrafficFilter.Matches
{
    public interface IMatchesFactory
    {
        IMatch GetInstance(string type, string match);
    }

    public class MatchesFactory : IMatchesFactory
    {
        private Dictionary<string, Type> _matchTypes;

        public IMatch GetInstance(string type, string match)
        {
            return Activator.CreateInstance(MatchTypes[type], match) as IMatch;
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