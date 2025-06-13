using System;

namespace TrafficFilter.Matches
{
    public class MatchStartsWith : MatchBase
    {
        public MatchStartsWith(string match, string group)
            : base(match, group)
        {
        }

        public override bool IsMatch(string source)
        {
            return source.StartsWith(Match, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}