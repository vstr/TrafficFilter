using System;

namespace TrafficFilter.Matches
{
    public abstract class MatchBase : IMatch
    {
        public MatchBase(string match, string group)
        {
            Match = match ?? throw new ArgumentNullException(nameof(match), "Match cannot be null.");
            Group = group ?? Guid.NewGuid().ToString();
        }

        public string Match { get; protected set; }
        public string Group { get; protected set; }

        public abstract bool IsMatch(string source);
    }
}