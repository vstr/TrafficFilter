namespace TrafficFilter.Matches
{
    public class MatchNullOrEmpty : IMatch
    {
        public MatchNullOrEmpty(string match)
        {
            Match = match;
        }

        public string Match { get; }

        public bool IsMatch(string source)
        {
            return string.IsNullOrEmpty(source);
        }
    }
}