namespace TrafficFilter.Matches
{
    public class MatchNullOrEmpty : MatchBase
    {
        public MatchNullOrEmpty(string match, string group) 
            : base(match, group)
        {
        }

        public override bool IsMatch(string source)
        {
            return string.IsNullOrEmpty(source);
        }
    }
}