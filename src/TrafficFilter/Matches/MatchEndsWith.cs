using System;

namespace TrafficFilter.Matches
{
    public class MatchEndsWith : IMatch
    {
        private readonly string _match;

        public MatchEndsWith(string match)
        {
            _match = match;
        }

        public string Match => _match;
        public bool IsMatch(string source)
        {
            return source.EndsWith(_match, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
