namespace TrafficFilter.RequestFiltering
{
    public interface IRequestFilter
    {
        bool IsEnabled { get; }
    }
}
