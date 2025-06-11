using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using TrafficFilter.Configuration;
using TrafficFilter.CoreFirewall;
using TrafficFilter.CoreFirewall.Configuration;
using TrafficFilter.Matches;
using Xunit;

namespace TrafficFilter.Tests
{
    public class FirewallRuleIpTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenMatchesFactoryIsNull()
        {
            var logger = Substitute.For<ILogger<Firewall>>();
            var options = Options.Create(new FirewallOptions { IsEnabled = true, BlockRules = [] });
            Assert.Throws<ArgumentNullException>(() => new Firewall(null, options, logger));
        }

        [Fact]
        public void FirewallRuleIp_Exact()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "Exact",
                Match = "192.168.1.2",
                RequestPart = "ip"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Connection.RemoteIpAddress.Returns(System.Net.IPAddress.Parse("192.168.1.2"));

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContext);

            Assert.True(isMatch, "Firewall rule should match the IP address in the context.");
        }

        [Fact]
        public void FirewallRuleIp_Regex()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "Regex",
                Match = @"192\.168\.\d+\.2",
                RequestPart = "ip"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Connection.RemoteIpAddress.Returns(System.Net.IPAddress.Parse("192.168.222.2"));

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContext);

            Assert.True(isMatch, "Firewall rule should match the IP address in the context.");
        }

        [Fact]
        public void FirewallRuleIp_Contains()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "Contains",
                Match = "168.1",
                RequestPart = "ip"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Connection.RemoteIpAddress.Returns(System.Net.IPAddress.Parse("192.168.1.2"));

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContext);

            Assert.True(isMatch, "Firewall rule should match the IP address in the context.");
        }

        [Fact]
        public void FirewallRuleIp_StartsWith()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "StartsWith",
                Match = "192.",
                RequestPart = "ip"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Connection.RemoteIpAddress.Returns(System.Net.IPAddress.Parse("192.168.1.2"));

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContext);

            Assert.True(isMatch, "Firewall rule should match the IP address in the context.");
        }

        [Fact]
        public void FirewallRuleIp_EndsWith()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "EndsWith",
                Match = ".2",
                RequestPart = "ip"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var httpContext = Substitute.For<HttpContext>();
            httpContext.Connection.RemoteIpAddress.Returns(System.Net.IPAddress.Parse("192.168.1.2"));

            var logger = Substitute.For<ILogger<Firewall>>();
            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContext);

            Assert.True(isMatch, "Firewall rule should match the IP address in the context.");
        }
    }
}