using System;

namespace TrafficFilter.Matches
{
    public class MatchStartsWith : IMatch
    {
        private readonly string _match;

        public MatchStartsWith(string match)
        {
            _match = match;
        }

        public string Match => _match;

        public bool IsMatch(string source)
        {
            return source.StartsWith(_match, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}