using System;

namespace TrafficFilter.Matches
{
    public class MatchEndsWith : MatchBase
    {
        public MatchEndsWith(string match, string group) 
            : base(match, group)
        {
        }

        public override bool IsMatch(string source)
        {
            return source.EndsWith(Match, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}