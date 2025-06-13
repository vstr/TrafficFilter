namespace TrafficFilter.Matches
{
    public interface IMatch
    {
        bool IsMatch(string source);

        string Match { get; }
        string Group { get; }
    }
}