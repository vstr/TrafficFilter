using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using TrafficFilter.Configuration;
using TrafficFilter.CoreRateLimiter;
using TrafficFilter.CoreRateLimiter.Configuration;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;
using TrafficFilter.Tests.Common;
using Xunit;

namespace TrafficFilter.Tests
{
    public class RateLimiterTests
    {
        [Fact]
        public void IsEnabledFalseReturnsIsLimitReachedFalse()
        {
            //Arrange
            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RateLimiterOptions>>();
            options.Value.Returns(new RateLimiterOptions()
            {
                IsEnabled = false
            });

            var logger = Substitute.For<ILogger<RateLimiter>>();

            var rateLimiter = new RateLimiter(options, matchesFactory, logger);

            var httpContext = new DefaultHttpContext();

            //Act
            var isLimitReached = rateLimiter.IsMatch(httpContext);

            //Assert
            isLimitReached.Should().BeFalse();
        }

        [Fact]
        public void WhitelistUrlsMatchedReturnsIsLimitReachedFalse()
        {
            //Arrange
            var matchesFactory = new MatchesFactory();

            var options = Substitute.For<IOptions<RateLimiterOptions>>();
            options.Value.Returns(new RateLimiterOptions()
            {
                IsEnabled = true,
                RequestsLimit = 1,
                WindowSeconds = 1,
                WhitelistRules = [ new RuleOptions()
                        {
                            MatchType = "Contains",
                            Match = ".mp4",
                            RequestPart = "Url"
                        }
                ]
            });

            var logger = Substitute.For<ILogger<RateLimiter>>();

            var rateLimiter = new RateLimiter(options, matchesFactory, logger);

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

            var options = Substitute.For<IOptions<RateLimiterOptions>>();
            options.Value.Returns(new RateLimiterOptions()
            {
                IsEnabled = true,
                RequestsLimit = 1,
                WindowSeconds = 1
            });

            var logger = Substitute.For<ILogger<RateLimiter>>();

            var rateLimiter = new RateLimiter(options, matchesFactory, logger);

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

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

            var ruleOptions = new RuleOptions
            {
                RequestPart = "Url",
                MatchType = "Contains",
                Match = ".mp3",
                Group = "TestGroup"
            };

            matchesFactory.GetInstance(ruleOptions).Returns(new MatchContains(".mp3", "TestGroup"));

            var options = Substitute.For<IOptions<RateLimiterOptions>>();
            options.Value.Returns(new RateLimiterOptions()
            {
                IsEnabled = true,
                RequestsLimit = 1,
                WindowSeconds = 1,
                WhitelistRules = [ new RuleOptions()
                        {
                            MatchType = "Contains",
                            Match = ".mp3",
                            RequestPart = "Url"
                        }
                ]
            });

            var logger = Substitute.For<ILogger<RateLimiter>>();

            var rateLimiter = new RateLimiter(options, matchesFactory, logger);

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

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

            var options = Substitute.For<IOptions<RateLimiterOptions>>();
            options.Value.Returns(new RateLimiterOptions()
            {
                IsEnabled = true,
                RequestsLimit = 1,
                WindowSeconds = 1
            });

            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("https", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

            var logger = Substitute.For<ILogger<RateLimiter>>();

            var rateLimiter = new RateLimiter(options, matchesFactory, logger);

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