using System;

namespace TrafficFilter.Matches
{
    public class MatchContains : IMatch
    {
        private readonly string _match;

        public MatchContains(string match)
        {
            _match = match;
        }

        public string Match => _match;

        public bool IsMatch(string source)
        {
            return source.Contains(_match, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
