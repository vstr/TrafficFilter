using System.Collections.Generic;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

using TrafficFilter.Configuration;
using TrafficFilter.Matches;
using TrafficFilter.RequestFiltering;

using Xunit;

namespace TrafficFilter.Tests
{
    public class RequestFilterHeadersTests
    {
        [Fact]
        public void MatchContainsOtherAgentReturnsOkRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterHeaders>>();

            var headerName = "User-Agent";

            var headerDictionary = Substitute.For<IHeaderDictionary>();
            headerDictionary.ContainsKey(headerName).Returns(true);
            headerDictionary[headerName].Returns(new Microsoft.Extensions.Primitives.StringValues("agent X-Bot here"));

            var match = @"y-bot";
            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("MatchContains", match).Returns(new MatchContains(match));

            var options = Substitute.For<IOptions<RequestFilterHeadersOptions>>();
            options.Value.Returns(new RequestFilterHeadersOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemHeader>() { new MatchItemHeader()
                    {
                        Header = headerName,
                        Type = "MatchContains",
                        Match = match
                    }
                }
            });

            var requestFilterHeaders = new RequestFilterHeaders(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterHeaders.IsBadRequest(headerDictionary);

            //Assert
            isBadRequest.Should().BeFalse();
        }

        [Fact]
        public void MatchContainsForbiddenUserAgentReturnsBadRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterHeaders>>();

            var headerName = "User-Agent";

            var headerDictionary = Substitute.For<IHeaderDictionary>();
            headerDictionary.ContainsKey(headerName).Returns(true);
            headerDictionary[headerName].Returns(new Microsoft.Extensions.Primitives.StringValues("agent X-Bot here"));

            var match = @"x-bot";
            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("MatchContains", match).Returns(new MatchContains(match));

            var options = Substitute.For<IOptions<RequestFilterHeadersOptions>>();
            options.Value.Returns(new RequestFilterHeadersOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemHeader>() { new MatchItemHeader()
                    {
                        Header = headerName,
                        Type = "MatchContains",
                        Match = match
                    }
                }
            });

            var requestFilterHeaders = new RequestFilterHeaders(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterHeaders.IsBadRequest(headerDictionary);

            //Assert
            isBadRequest.Should().BeTrue();
        }

        [Fact]
        public void MatchContainsHeaderMissingReturnsBadRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterHeaders>>();

            var headerDictionary = Substitute.For<IHeaderDictionary>();

            var match = @"x-bot";
            var matchesFactory = Substitute.For<IMatchesFactory>();
            matchesFactory.GetInstance("MatchContains", match).Returns(new MatchContains(match));

            var options = Substitute.For<IOptions<RequestFilterHeadersOptions>>();
            options.Value.Returns(new RequestFilterHeadersOptions()
            {
                IsEnabled = true,
                Matches = new List<MatchItemHeader>() { new MatchItemHeader()
                    {
                        Header = "User-Agent",
                        Type = "MatchContains",
                        Match = match
                    }
                }
            });

            var requestFilterHeaders = new RequestFilterHeaders(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterHeaders.IsBadRequest(headerDictionary);

            //Assert
            isBadRequest.Should().BeTrue();
        }

        [Fact]
        public void IsEnabledFalseReturnsOkRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterHeaders>>();

            var headerDictionary = Substitute.For<IHeaderDictionary>();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterHeadersOptions>>();
            options.Value.Returns(new RequestFilterHeadersOptions()
            {
                IsEnabled = false
            });

            var requestFilterHeaders = new RequestFilterHeaders(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterHeaders.IsBadRequest(headerDictionary);

            //Assert
            isBadRequest.Should().BeFalse();
        }

        [Fact]
        public void HeadersNullReturnsBadRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RequestFilterHeaders>>();

            var matchesFactory = Substitute.For<IMatchesFactory>();

            var options = Substitute.For<IOptions<RequestFilterHeadersOptions>>();
            options.Value.Returns(new RequestFilterHeadersOptions()
            {
                IsEnabled = true
            });

            var requestFilterHeaders = new RequestFilterHeaders(options, matchesFactory, logger);

            //Act
            var isBadRequest = requestFilterHeaders.IsBadRequest(null);

            //Assert
            isBadRequest.Should().BeTrue();
        }
    }
}
