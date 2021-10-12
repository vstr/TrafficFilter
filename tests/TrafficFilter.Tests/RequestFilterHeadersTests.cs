using System.Collections.Generic;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using NSubstitute;

using TrafficFilter.Configuration;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;
using TrafficFilter.RequestFilters;
using TrafficFilter.Tests.Common;

using Xunit;

namespace TrafficFilter.Tests
{
    public class RequestFilterHeadersTests
    {
        [Fact]
        public void ContainsNotMatchingDifferentUserAgents()
        {
            //Arrange
            var headerName = "User-Agent";

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();
            httpContextAccessor.HttpContext.Request.Headers.ContainsKey(headerName).Returns(true);
            httpContextAccessor.HttpContext.Request.Headers[headerName].Returns(new Microsoft.Extensions.Primitives.StringValues("agent X-Bot here"));

            var match = @"y-bot";
            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("Contains", match).Returns(new MatchContains(match));

            var options = Substitute.For<IOptions<RequestFilterHeadersOptions>>();
            options.Value.Returns(new RequestFilterHeadersOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemHeader>() { new MatchItemHeader()
                    {
                        Header = headerName,
                        Type = "Contains",
                        Match = match
                    }
                }
            });

            var requestFilterHeaders = new RequestFilterHeaders(options, matchesFactory);

            //Act
            var isMatch = requestFilterHeaders.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeFalse();
            requestFilterHeaders.Order.Should().Be(2);
        }

        [Fact]
        public void ContainsMatchesForbiddenUserAgent()
        {
            //Arrange
            var headerName = "User-Agent";

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();
            httpContextAccessor.HttpContext.Request.Headers.ContainsKey(headerName).Returns(true);
            httpContextAccessor.HttpContext.Request.Headers[headerName].Returns(new Microsoft.Extensions.Primitives.StringValues("agent X-Bot here"));

            var match = @"x-bot";
            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("Contains", match).Returns(new MatchContains(match));

            var options = Substitute.For<IOptions<RequestFilterHeadersOptions>>();
            options.Value.Returns(new RequestFilterHeadersOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemHeader>() { new MatchItemHeader()
                    {
                        Header = headerName,
                        Type = "Contains",
                        Match = match
                    }
                }
            });

            var requestFilterHeaders = new RequestFilterHeaders(options, matchesFactory);

            //Act
            var isMatch = requestFilterHeaders.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeTrue();
        }

        [Fact]
        public void MissingHeaderConsideredAsMatch()
        {
            //Arrange
            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

            var match = @"x-bot";
            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("Contains", match).Returns(new MatchContains(match));

            var options = Substitute.For<IOptions<RequestFilterHeadersOptions>>();
            options.Value.Returns(new RequestFilterHeadersOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemHeader>() { new MatchItemHeader()
                    {
                        Header = "User-Agent",
                        Type = "Contains",
                        Match = match
                    }
                }
            });

            var requestFilterHeaders = new RequestFilterHeaders(options, matchesFactory);

            //Act
            var isMatch = requestFilterHeaders.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeTrue();
        }

        [Fact]
        public void IsEnabledFalseResultsInNoMatch()
        {
            //Arrange
            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterHeadersOptions>>();
            options.Value.Returns(new RequestFilterHeadersOptions()
            {
                IsEnabled = false
            });

            var requestFilterHeaders = new RequestFilterHeaders(options, matchesFactory);

            //Act
            var isMatch = requestFilterHeaders.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeFalse();
        }

        [Fact]
        public void HeadersNullMatches()
        {
            //Arrange
            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();
            _ = httpContextAccessor.HttpContext.Request.Headers.Returns((IHeaderDictionary)null);

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterHeadersOptions>>();
            options.Value.Returns(new RequestFilterHeadersOptions()
            {
                IsEnabled = true
            });

            var requestFilterHeaders = new RequestFilterHeaders(options, matchesFactory);

            //Act
            var isMatch = requestFilterHeaders.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeTrue();
        }
    }
}