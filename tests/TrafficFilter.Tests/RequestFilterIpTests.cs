using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Collections.Generic;
using TrafficFilter.Configuration;
using TrafficFilter.Matches;
using TrafficFilter.RequestFilters;
using TrafficFilter.Tests.Common;
using Xunit;

namespace TrafficFilter.Tests
{
    public class RequestFilterIpTests
    {
        [Fact]
        public void IsEnabledFalseReturnsNoMatch()
        {
            var httpContext = TestHelper.BuildHttpContextAccessor("http", "192.168.1.1", "/").HttpContext;
            var options = Substitute.For<IOptions<RequestFilterIpOptions>>();
            options.Value.Returns(new RequestFilterIpOptions { IsEnabled = false, Matches = new List<MatchItemIp>() });
            var logger = Substitute.For<ILogger<RequestFilterIp>>();
            var matchesFactory = Substitute.For<IMatchesFactory>();

            var filter = new RequestFilterIp(options, matchesFactory, logger);

            filter.IsMatch(httpContext).Should().BeFalse();
        }

        [Theory]
        [InlineData("192.168.*.*", "192.168.10.100", true)]
        [InlineData("192.168.*.*", "192.210.10.150", false)]
        [InlineData("192.168.1.*", "192.168.1.200", true)]
        [InlineData("192.168.1.*", "192.168.2.200", false)]
        [InlineData("192.168.1.15", "192.168.1.15", true)]
        [InlineData("192.168.1.15", "192.168.1.16", false)]
        public void IpMatchCheck(string match, string address, bool isMatch)
        {
            var ip = address;
            var httpContext = TestHelper.BuildHttpContextAccessor("http", ip, "/").HttpContext;
            var options = Substitute.For<IOptions<RequestFilterIpOptions>>();
            options.Value.Returns(new RequestFilterIpOptions
            {
                IsEnabled = true,
                Matches = new List<MatchItemIp> { new MatchItemIp { Match = match } }
            });
            var logger = Substitute.For<ILogger<RequestFilterIp>>();
            var matchesFactory = Substitute.For<IMatchesFactory>();

            var filter = new RequestFilterIp(options, matchesFactory, logger);

            filter.IsMatch(httpContext).Should().Be(isMatch);
        }

        [Fact]
        public void EmptyMatchesReturnsFalse()
        {
            var httpContext = TestHelper.BuildHttpContextAccessor("http", "127.0.0.1", "/").HttpContext;
            var options = Substitute.For<IOptions<RequestFilterIpOptions>>();
            options.Value.Returns(new RequestFilterIpOptions
            {
                IsEnabled = true,
                Matches = new List<MatchItemIp>()
            });
            var logger = Substitute.For<ILogger<RequestFilterIp>>();
            var matchesFactory = Substitute.For<IMatchesFactory>();

            var filter = new RequestFilterIp(options, matchesFactory, logger);

            filter.IsMatch(httpContext).Should().BeFalse();
        }
    }
}