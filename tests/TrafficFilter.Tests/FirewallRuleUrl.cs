using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using TrafficFilter.Configuration;
using TrafficFilter.CoreFirewall;
using TrafficFilter.CoreFirewall.Configuration;
using TrafficFilter.Matches;
using TrafficFilter.Tests.Common;
using Xunit;

namespace TrafficFilter.Tests
{
    public class FirewallRuleUrlTests
    {
        [Fact]
        public void FirewallRuleUrl_Exact()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "Exact",
                Match = "https://192.168.0.1/basepath/home/intro.mp4?sort=desc",
                RequestPart = "Url"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4", "?sort=desc");

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContextAccessor.HttpContext);

            Assert.True(isMatch, "Firewall rule should match the Url in the context.");
        }

        [Fact]
        public void FirewallRuleUrl_Regex()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "Regex",
                Match = @"https:\/\/192\.\d+\.0\.1/basepath/home/intro\.mp4\?sort=desc",
                RequestPart = "Url"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4", "?sort=desc");

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContextAccessor.HttpContext);

            Assert.True(isMatch, "Firewall rule should match the Url in the context.");
        }

        [Fact]
        public void FirewallRuleUrl_Contains()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "Contains",
                Match = "intro",
                RequestPart = "Url"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4", "?sort=desc");

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContextAccessor.HttpContext);

            Assert.True(isMatch, "Firewall rule should match the Url in the context.");
        }

        [Fact]
        public void FirewallRuleUrl_StartsWith()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "StartsWith",
                Match = "https",
                RequestPart = "Url"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4", "?sort=desc");

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContextAccessor.HttpContext);

            Assert.True(isMatch, "Firewall rule should match the Url in the context.");
        }

        [Fact]
        public void FirewallRuleUrl_EndsWith()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "EndsWith",
                Match = "desc",
                RequestPart = "Url"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4", "?sort=desc");

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContextAccessor.HttpContext);

            Assert.True(isMatch, "Firewall rule should match the Url in the context.");
        }
    }
}