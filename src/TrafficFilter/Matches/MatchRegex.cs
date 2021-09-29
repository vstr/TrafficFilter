using System.Text.RegularExpressions;

namespace TrafficFilter.Matches
{
    public class MatchRegex : IMatch
    {
        private readonly Regex _regex;
        private readonly string _pattern;

        public MatchRegex(string pattern)
        {
            _pattern = pattern;
            _regex = new Regex(pattern);
        }

        public string Match => _pattern;

        public bool IsMatch(string source)
        {
            return _regex.IsMatch(source);
        }
    }
}
