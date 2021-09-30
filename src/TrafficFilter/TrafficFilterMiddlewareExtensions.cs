using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TrafficFilter.Configuration;
using TrafficFilter.Matches;
using TrafficFilter.RateLimit;
using TrafficFilter.RequestFiltering;

namespace TrafficFilter
{
    public static class TrafficFilterMiddlewareExtensions
    {
        public static IApplicationBuilder UseTrafficFilter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TrafficFilterMiddleware>();
        }

        public static IServiceCollection AddTrafficFilter(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddSingleton<IIpBlacklist, IpBlacklist>();
            _ = services.AddSingleton<IRequestFilterUrl, RequestFilterUrl>();
            _ = services.AddSingleton<IRequestFilterHeaders, RequestFilterHeaders>();
            _ = services.AddSingleton<IRateLimiter, RateLimiter>();
            _ = services.AddSingleton<IMatchesFactory, MatchesFactory>();

            _ = services.AddOptions();

            _ = services.Configure<TrafficFilterOptions>(configuration.GetSection(TrafficFilterOptions.TrafficFilter));

            _ = services.Configure<RequestFilterUrlOptions>(configuration
                .GetSection(TrafficFilterOptions.TrafficFilter)
                .GetSection(RequestFilterUrlOptions.RequestFilterUrl));

            _ = services.Configure<RequestFilterHeadersOptions>(configuration
                .GetSection(TrafficFilterOptions.TrafficFilter)
                .GetSection(RequestFilterHeadersOptions.RequestFilterHeaders));

            _ = services.Configure<RateLimiterOptions>(configuration
                .GetSection(TrafficFilterOptions.TrafficFilter)
                .GetSection(RateLimiterOptions.RateLimiter));

            return services;
        }
    }
}