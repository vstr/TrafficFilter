using System;

namespace TrafficFilter.Matches
{
    public class MatchContains : MatchBase
    {
        public MatchContains(string match, string group)
            : base(match, group)
        {
        }

        public override bool IsMatch(string source)
        {
            return source.Contains(Match, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}