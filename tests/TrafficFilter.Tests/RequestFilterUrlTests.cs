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
    public class RequestFilterUrlTests
    {
        [Theory]
        [InlineData("http", "127.0.0.1")]
        [InlineData("https", "127.0.0.1")]
        public void RegexOnIPAdressReturnsMatch(string scheme, string ipAddress)
        {
            //Arrange
            var httpContextAccessor = TestHelper.BuildHttpContextAccessor(scheme, ipAddress, "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

            var pattern = @"https?:\/\/[\d*\.*]+";
            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("Regex", pattern).Returns(new MatchRegex(pattern));

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemUrl>() { new MatchItemUrl()
                    {
                        Type = "Regex",
                        Match = pattern
                    }
                }
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory);

            //Act
            var isMatch = requestFilterUrl.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeTrue();
            requestFilterUrl.Order.Should().Be(1);
        }

        [Fact]
        public void EmptyMatchesReturnsNoMatch()
        {
            //Arrange
            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("htt", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory);

            //Act
            var isMatch = requestFilterUrl.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeFalse();
        }

        [Fact]
        public void EmptyMatchesProvidedReturnsNoMatch()
        {
            //Arrange
            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("htt", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemUrl>()
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory);

            //Act
            var isMatch = requestFilterUrl.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeFalse();
        }

        [Fact]
        public void StartsWithReturnsMatch()
        {
            //Arrange
            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("http", "192.168.0.1", "/home/intro.mp4");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("StartsWith", "http:").Returns(new MatchStartsWith("http:"));

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemUrl>() { new MatchItemUrl()
                        {
                            Type = "StartsWith",
                            Match = "http:"
                        }
                }
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory);

            //Act
            var isMatch = requestFilterUrl.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeTrue();
        }

        [Fact]
        public void EndsWithReturnsMatch()
        {
            //Arrange
            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("http", "192.168.0.1", "/home/intro.mp4", "?a=15");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("EndsWith", "a=15").Returns(new MatchEndsWith("a=15"));

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemUrl>() { new MatchItemUrl()
                        {
                            Type = "EndsWith",
                            Match = "a=15"
                        }
                }
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory);

            //Act
            var isMatch = requestFilterUrl.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeTrue();
        }

        [Fact]
        public void ContainsReturnsMatch()
        {
            //Arrange
            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("http", "192.168.0.1", "/home/intro.php", "?a=15");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("Contains", ".php").Returns(new MatchContains(".php"));

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemUrl>() { new MatchItemUrl()
                        {
                            Type = "Contains",
                            Match = ".php"
                        }
                }
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory);

            //Act
            var isMatch = requestFilterUrl.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeTrue();
        }

        [Fact]
        public void ContainsMissingMatchReturnsNoMatch()
        {
            //Arrange
            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("http", "192.168.0.1", "/home/intro.php", "?a=15");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("MatchContains", "abc").Returns(new MatchContains("abc"));

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemUrl>() { new MatchItemUrl()
                        {
                            Type = "MatchContains",
                            Match = "abc"
                        }
                }
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory);

            //Act
            var isMatch = requestFilterUrl.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeFalse();
        }

        [Fact]
        public void IsEnabledFalseReturnsNoMatch()
        {
            //Arrange
            var httpContextAccessor = TestHelper.BuildHttpContextAccessor("http", "192.168.0.1", "/home/intro.php", "?a=15");
            _ = httpContextAccessor.HttpContext.GetIPAddress();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = false
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory);

            //Act
            var isMatch = requestFilterUrl.IsMatch(httpContextAccessor.HttpContext);

            //Assert
            isMatch.Should().BeFalse();
        }

        [Fact]
        public void NullRequestUrlReturnsMatch()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory);

            //Act
            var isMatch = requestFilterUrl.IsMatch(httpContext);

            //Assert
            isMatch.Should().BeTrue();
        }
    }
}