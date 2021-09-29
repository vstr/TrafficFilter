using System.Collections.Generic;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

using TrafficFilter.Configuration;
using TrafficFilter.Matches;
using TrafficFilter.RequestFiltering;

using Xunit;

namespace TrafficFilter.Tests
{
    public class RequestFilterUrlTests
    {
        [Theory]
        [InlineData(@"http://127.0.0.1")]
        [InlineData(@"https://127.0.0.1")]
        public void MatchRegexOnIPAdressReturnsBadRequest(string url)
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterUrl>>();

            var pattern = @"https?:\/\/[\d*\.*]+";
            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("MatchRegex", pattern).Returns(new MatchRegex(pattern));

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemUrl>() { new MatchItemUrl()
                    {
                        Type = "MatchRegex",
                        Match = pattern
                    }
                }
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterUrl.IsBadRequest(url);

            //Assert
            isBadRequest.Should().BeTrue();
        }

        [Fact]
        public void EmptyMatchesReturnsOkRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterUrl>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterUrl.IsBadRequest(@"htt://192.168.0.1");

            //Assert
            isBadRequest.Should().BeFalse();
        }

        [Fact]
        public void EmptyMatchesProvidedReturnsOkRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterUrl>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemUrl>()
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterUrl.IsBadRequest(@"htt://192.168.0.1");

            //Assert
            isBadRequest.Should().BeFalse();
        }

        [Fact]
        public void MatchStartsWithReturnsBadRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterUrl>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("MatchStartsWith", "htt:").Returns(new MatchStartsWith("htt:"));

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemUrl>() { new MatchItemUrl()
                        {
                            Type = "MatchStartsWith",
                            Match = "htt:"
                        }
                }
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterUrl.IsBadRequest(@"htt://192.168.0.1/index.php?a=15");

            //Assert
            isBadRequest.Should().BeTrue();
        }

        [Fact]
        public void MatchEndsWithReturnsBadRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterUrl>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("MatchEndsWith", "a=15").Returns(new MatchEndsWith("a=15"));

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemUrl>() { new MatchItemUrl()
                        {
                            Type = "MatchEndsWith",
                            Match = "a=15"
                        }
                }
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterUrl.IsBadRequest(@"htt://192.168.0.1/index.php?a=15");

            //Assert
            isBadRequest.Should().BeTrue();
        }

        [Fact]
        public void MatchContainsReturnsBadRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterUrl>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("MatchContains", ".php").Returns(new MatchContains(".php"));

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemUrl>() { new MatchItemUrl()
                        {
                            Type = "MatchContains",
                            Match = ".php"
                        }
                }
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterUrl.IsBadRequest(@"htt://192.168.0.1/index.php?a=15");

            //Assert
            isBadRequest.Should().BeTrue();
        }

        [Fact]
        public void MatchContainsMissingMatchReturnsOkRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterUrl>>();

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

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterUrl.IsBadRequest(@"htt://192.168.0.1/index.php?a=15");

            //Assert
            isBadRequest.Should().BeFalse();
        }

        [Fact]
        public void IsEnabledFalseReturnsOkRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterUrl>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("MatchContains", "abc").Returns(new MatchContains("abc"));

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = false
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterUrl.IsBadRequest(@"htt://192.168.0.1/index.php?a=15");

            //Assert
            isBadRequest.Should().BeFalse();
        }

        [Fact]
        public void NullRequestUrlReturnsBadRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterUrl>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterUrlOptions>>();
            options.Value.Returns(new RequestFilterUrlOptions()
            {
                IsEnabled = true
            });

            var requestFilterUrl = new RequestFilterUrl(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterUrl.IsBadRequest(null);

            //Assert
            isBadRequest.Should().BeTrue();
        }
    }
}
