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
    public class RequestFilterRateLimiterTests
    {
        [Fact]
        public void IsEnabledFalseReturnsIsLimitReachedFalse()
        {
            //Arrange
            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterRateLimiterOptions>>();
            options.Value.Returns(new RequestFilterRateLimiterOptions()
            {
                IsEnabled = false
            });

            var rateLimiter = new RequestFilterRateLimiter(options, matchesFactory);

            var httpContext = new DefaultHttpContext();

            //Act
            var isLimitReached = rateLimiter.IsMatch(httpContext);

            //Assert
            isLimitReached.Should().BeFalse();
            rateLimiter.Order.Should().Be(3);
        }

        [Fact]
        public void WhitelistUrlsMatchedReturnsIsLimitReachedFalse()
        {
            //Arrange
            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("Contains", ".mp4").Returns(new MatchContains(".mp4"));

            var options = Substitute.For<IOptions<RequestFilterRateLimiterOptions>>();
            options.Value.Returns(new RequestFilterRateLimiterOptions()
            {
                IsEnabled = true,
                RateLimiterRequestLimit = 1,
                RateLimiterWindowSeconds = 1,
                WhitelistUrls = new List<MatchItemUrl>() { new MatchItemUrl()
                        {
                            Type = "Contains",
                            Match = ".mp4"
                        }
                }
            });

            var rateLimiter = new RequestFilterRateLimiter(options, matchesFactory);

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");

            //Act
            _ = rateLimiter.IsMatch(httpContextAccessor.HttpContext);
            _ = rateLimiter.IsMatch(httpContextAccessor.HttpContext);
            _ = rateLimiter.IsMatch(httpContextAccessor.HttpContext);
            var isLimitReached = rateLimiter.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isLimitReached.Should().BeFalse();
        }

        [Fact]
        public void WhitelistUrlsEmptyReturnsIsLimitReachedTrue()
        {
            //Arrange
            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterRateLimiterOptions>>();
            options.Value.Returns(new RequestFilterRateLimiterOptions()
            {
                IsEnabled = true,
                RateLimiterRequestLimit = 1,
                RateLimiterWindowSeconds = 1
            });

            var rateLimiter = new RequestFilterRateLimiter(options, matchesFactory);

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress(false);

            //Act
            _ = rateLimiter.IsMatch(httpContextAccessor.HttpContext);
            _ = rateLimiter.IsMatch(httpContextAccessor.HttpContext);
            _ = rateLimiter.IsMatch(httpContextAccessor.HttpContext);
            var isLimitReached = rateLimiter.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isLimitReached.Should().BeTrue();
        }

        [Fact]
        public void WhitelistUrlsNotMatchedReturnsIsLimitReachedTrue()
        {
            //Arrange
            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("Contains", ".mp3").Returns(new MatchContains(".mp3"));

            var options = Substitute.For<IOptions<RequestFilterRateLimiterOptions>>();
            options.Value.Returns(new RequestFilterRateLimiterOptions()
            {
                IsEnabled = true,
                RateLimiterRequestLimit = 1,
                RateLimiterWindowSeconds = 1,
                WhitelistUrls = new List<MatchItemUrl>() { new MatchItemUrl()
                        {
                            Type = "Contains",
                            Match = ".mp3"
                        }
                }
            });

            var rateLimiter = new RequestFilterRateLimiter(options, matchesFactory);

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress(false);

            //Act
            _ = rateLimiter.IsMatch(httpContextAccessor.HttpContext);
            _ = rateLimiter.IsMatch(httpContextAccessor.HttpContext);
            _ = rateLimiter.IsMatch(httpContextAccessor.HttpContext);
            var isLimitReached = rateLimiter.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isLimitReached.Should().BeTrue();
        }

        [Fact]
        public void IsFullReturnsIsLimitReachedTrue()
        {
            //Arrange
            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterRateLimiterOptions>>();
            options.Value.Returns(new RequestFilterRateLimiterOptions()
            {
                IsEnabled = true,
                RateLimiterRequestLimit = 1,
                RateLimiterWindowSeconds = 1
            });

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress(false);

            var rateLimiter = new RequestFilterRateLimiter(options, matchesFactory);

            //Act
            var isLimitReached = rateLimiter.IsMatch(httpContextAccessor.HttpContext);
            //Assert
            isLimitReached.Should().BeFalse();

            //Act
            isLimitReached = rateLimiter.IsMatch(httpContextAccessor.HttpContext);
            //Assert
            isLimitReached.Should().BeTrue();
        }
    }
}