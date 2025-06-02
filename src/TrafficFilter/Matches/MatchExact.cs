using System;

namespace TrafficFilter.Matches
{
    public class MatchExact : IMatch
    {
        private readonly string _match;

        public MatchExact(string match)
        {
            _match = match;
        }

        public string Match => _match;

        public bool IsMatch(string source)
        {
            return string.Equals(source, _match, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}