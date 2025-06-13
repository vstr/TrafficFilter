using System.Text.RegularExpressions;

namespace TrafficFilter.Matches
{
    public class MatchRegex : MatchBase
    {
        private readonly Regex _regex;

        public MatchRegex(string match, string group)
            : base(match, group)
        {
            _regex = new Regex(Match, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public override bool IsMatch(string source)
        {
            return _regex.IsMatch(source);
        }
    }
}