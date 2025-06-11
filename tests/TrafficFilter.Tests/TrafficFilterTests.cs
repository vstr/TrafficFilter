using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using TrafficFilter.Configuration;
using TrafficFilter.Core;
using TrafficFilter.CoreFirewall;
using TrafficFilter.CoreFirewall.Configuration;
using TrafficFilter.CoreRateLimiter;
using TrafficFilter.Matches;
using Xunit;

namespace TrafficFilter.Tests
{
    public class TrafficFilterTests
    {
        [Fact]
        public void NotInBlacklistAndNoMatchReturnsAllowed()
        {
            //Arrange
            var ipAddress = IPAddress.Parse("192.168.0.1");

            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress= ipAddress
                }
            };

            var ipBlackList = Substitute.For<IIpBlacklist>();
            ipBlackList.IsInBlacklist(ipAddress).Returns(false);

            var firewall = Substitute.For<IFirewall>();
            var rateLimiter = Substitute.For<IRateLimiter>();

            var options = Substitute.For<IOptions<TrafficFilterOptions>>();
            options.Value.Returns(new TrafficFilterOptions()
            {
            });

            var logger = Substitute.For<ILogger<TrafficFilter>>();

            var trafficFilter = new TrafficFilter(ipBlackList, firewall, rateLimiter, options, logger);

            //Act
            var isAllowed = trafficFilter.IsAllowed(httpContext);

            //Assert
            isAllowed.Should().BeTrue();
            trafficFilter.TrafficFilterOptions.Should().NotBeNull();
        }

        [Fact]
        public void InBlacklistReturnsNotAllowed()
        {
            //Arrange
            var ipAddress = IPAddress.Parse("192.168.0.1");

            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress= ipAddress
                }
            };

            var ipBlackList = Substitute.For<IIpBlacklist>();
            ipBlackList.IsInBlacklist(ipAddress).Returns(true);

            var firewall = Substitute.For<IFirewall>();

            var rateLimiter = Substitute.For<IRateLimiter>();

            var options = Substitute.For<IOptions<TrafficFilterOptions>>();
            options.Value.Returns(new TrafficFilterOptions()
            {
            });

            var logger = Substitute.For<ILogger<TrafficFilter>>();

            var trafficFilter = new TrafficFilter(ipBlackList, firewall, rateLimiter, options, logger);

            //Act
            var isAllowed = trafficFilter.IsAllowed(httpContext);

            //Assert
            isAllowed.Should().BeFalse();
        }

        [Fact]
        public void NotInBlacklistAndMatchReturnsNotAllowed()
        {
            //Arrange
            var ipAddress = IPAddress.Parse("192.168.0.1");

            var ruleOptions = new RuleOptions
            {
                MatchType = "Exact",
                Match = "192.168.0.1",
                RequestPart = "ip"
            };
            var firewallOptions = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();
            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, firewallOptions, logger);

            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress = ipAddress
                }
            };

            var ipBlackList = Substitute.For<IIpBlacklist>();
            ipBlackList.IsInBlacklist(ipAddress).Returns(false);

            var rateLimiter = Substitute.For<IRateLimiter>();

            var options = Substitute.For<IOptions<TrafficFilterOptions>>();
            options.Value.Returns(new TrafficFilterOptions()
            {
            });

            var loggerTf = Substitute.For<ILogger<TrafficFilter>>();

            var trafficFilter = new TrafficFilter(ipBlackList, firewall, rateLimiter, options, loggerTf);

            //Act
            var isAllowed = trafficFilter.IsAllowed(httpContext);

            //Assert
            isAllowed.Should().BeFalse();
        }
    }
}