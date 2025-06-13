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
    public class RuleGroupTests
    {
        [Fact]
        public void SameGroup_AllRulesThatMatch_ShouldReturnTrue()
        {
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules =
                    [
                        new RuleOptions {
                            MatchType = "Exact",
                            Match = "https://192.168.0.1/basepath/home/intro.mp4?sort=desc",
                            RequestPart = "Url",
                            Group = "TestGroup1"
                        },
                        new RuleOptions {
                            MatchType = "EndsWith",
                            Match = "desc",
                            RequestPart = "Url",
                            Group = "TestGroup1"
                        },
                    ]
            });

            var logger = Substitute.For<ILogger<Firewall>>();

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4", "?sort=desc");

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContextAccessor.HttpContext);

            Assert.True(isMatch);
        }

        [Fact]
        public void SameGroup_NotAllRulesMatch_ShouldReturnFalse()
        {
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules =
                    [
                        new RuleOptions {
                            MatchType = "Exact",
                            Match = "https://192.168.0.1/basepath/home/intro.mp4?sort=desc",
                            RequestPart = "Url",
                            Group = "TestGroup1"
                        },
                        new RuleOptions {
                            MatchType = "EndsWith",
                            Match = "desc1111111",
                            RequestPart = "Url",
                            Group = "TestGroup1"
                        },
                    ]
            });

            var logger = Substitute.For<ILogger<Firewall>>();

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4", "?sort=desc");

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContextAccessor.HttpContext);

            Assert.False(isMatch);
        }
    }
}