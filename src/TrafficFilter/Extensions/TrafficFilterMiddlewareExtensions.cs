using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrafficFilter.Configuration;
using TrafficFilter.Core;
using TrafficFilter.CoreFirewall;
using TrafficFilter.CoreFirewall.Configuration;
using TrafficFilter.CoreRateLimiter;
using TrafficFilter.CoreRateLimiter.Configuration;
using TrafficFilter.Matches;

namespace TrafficFilter.Extensions
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

            _ = services.AddSingleton<IRateLimiter, RateLimiter>();
            _ = services.AddSingleton<IFirewall, Firewall>();

            _ = services.AddOptions();

            _ = services.Configure<TrafficFilterOptions>(configuration
                .GetSection(TrafficFilterOptions.TrafficFilter));

            _ = services.Configure<FirewallOptions>(configuration
                .GetSection(TrafficFilterOptions.TrafficFilter)
                .GetSection(FirewallOptions.SectionName));

            _ = services.Configure<RateLimiterOptions>(configuration
                .GetSection(TrafficFilterOptions.TrafficFilter)
                .GetSection(RateLimiterOptions.SectionName));

            return services;
        }
    }
}