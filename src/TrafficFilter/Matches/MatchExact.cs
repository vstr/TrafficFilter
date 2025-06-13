using System;

namespace TrafficFilter.Matches
{
    public class MatchExact : MatchBase
    {
        public MatchExact(string match, string group) 
            : base(match, group)
        {
        }

        public override bool IsMatch(string source)
        {
            return string.Equals(source.Trim(), Match, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}