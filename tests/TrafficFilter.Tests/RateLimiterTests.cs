using System.Collections.Generic;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

using TrafficFilter.Configuration;
using TrafficFilter.Matches;
using TrafficFilter.RateLimit;

using Xunit;

namespace TrafficFilter.Tests
{

    public class RateLimiterTests
    {
        [Fact]
        public void IsEnabledFalseReturnsIsLimitReachedFalse()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RateLimiter>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RateLimiterOptions>>();
            options.Value.Returns(new RateLimiterOptions()
            {
                IsEnabled = false
            });

            var rateLimiter = new RateLimiter(options, matchesFactory, logger);

            //Act
            var isLimitReached = rateLimiter.IsLimitReached("192.168.0.1", "/home");

            //Assert
            isLimitReached.Should().BeFalse();
        }

        [Fact]
        public void SkipUrlsMatchedReturnsIsLimitReachedFalse()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RateLimiter>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("MatchContains", ".mp4").Returns(new MatchContains(".mp4"));

            var options = Substitute.For<IOptions<RateLimiterOptions>>();
            options.Value.Returns(new RateLimiterOptions()
            {
                IsEnabled = true,
                SkipUrls = new List<MatchItemUrl>() { new MatchItemUrl()
                        {
                            Type = "MatchContains",
                            Match = ".mp4"
                        }
                }
            });

            var rateLimiter = new RateLimiter(options, matchesFactory, logger);

            //Act
            var isLimitReached = rateLimiter.IsLimitReached("192.168.0.1", "/home/intro.mp4?v=1");

            //Assert
            isLimitReached.Should().BeFalse();
        }

        [Fact]
        public void SkipUrlsNotMatchedReturnsIsLimitReachedFalse()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RateLimiter>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("MatchContains", ".mp4").Returns(new MatchContains(".mp4"));

            var options = Substitute.For<IOptions<RateLimiterOptions>>();
            options.Value.Returns(new RateLimiterOptions()
            {
                IsEnabled = true,
                SkipUrls = new List<MatchItemUrl>() { new MatchItemUrl()
                        {
                            Type = "MatchContains",
                            Match = ".mp3"
                        }
                }
            });

            var rateLimiter = new RateLimiter(options, matchesFactory, logger);

            //Act
            var isLimitReached = rateLimiter.IsLimitReached("192.168.0.1", "/home/intro.mp4?v=1");

            //Assert
            isLimitReached.Should().BeFalse();
        }

        [Fact]
        public void IsFullReturnsIsLimitReachedTrue()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RateLimiter>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RateLimiterOptions>>();
            options.Value.Returns(new RateLimiterOptions()
            {
                IsEnabled = true,
                RateLimiterRequestLimit = 1,
                RateLimiterWindowSeconds = 1
            });

            var rateLimiter = new RateLimiter(options, matchesFactory, logger);

            //Act
            var isLimitReached = rateLimiter.IsLimitReached("192.168.0.1", "/home");
            //Assert
            isLimitReached.Should().BeFalse();

            //Act
            isLimitReached = rateLimiter.IsLimitReached("192.168.0.1", "/home");
            //Assert
            isLimitReached.Should().BeTrue();
        }
    }
}
