using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TrafficFilter.Configuration;
using TrafficFilter.Core;
using TrafficFilter.Matches;
using TrafficFilter.RequestFilters;

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
            _ = services.AddSingleton<IMatchesFactory, MatchesFactory>();
            _ = services.AddSingleton<ITrafficFilter, TrafficFilter>();
            _ = services.AddSingleton<IRequestFiltersFactory, RequestFiltersFactory>();

            _ = services.AddSingleton<RequestFilterUrl>();
            _ = services.AddSingleton<RequestFilterHeaders>();
            _ = services.AddSingleton<RequestFilterRateLimiterGlobal>();
            _ = services.AddSingleton<RequestFilterRateLimiterByPath>();

            _ = services.AddOptions();

            _ = services.Configure<TrafficFilterOptions>(configuration.GetSection(TrafficFilterOptions.TrafficFilter));

            _ = services.Configure<RequestFilterUrlOptions>(configuration
                .GetSection(TrafficFilterOptions.TrafficFilter)
                .GetSection(RequestFilterUrlOptions.RequestFilterUrl));

            _ = services.Configure<RequestFilterHeadersOptions>(configuration
                .GetSection(TrafficFilterOptions.TrafficFilter)
                .GetSection(RequestFilterHeadersOptions.RequestFilterHeaders));

            _ = services.Configure<RequestFilterRateLimiterGlobalOptions>(configuration
                .GetSection(TrafficFilterOptions.TrafficFilter)
                .GetSection(RequestFilterRateLimiterGlobalOptions.RateLimiterGlobal));

            _ = services.Configure<RequestFilterRateLimiterByPathOptions>(configuration
                .GetSection(TrafficFilterOptions.TrafficFilter)
                .GetSection(RequestFilterRateLimiterByPathOptions.RateLimiterByPath));

            return services;
        }
    }
}