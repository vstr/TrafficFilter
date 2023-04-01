using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

using System.Collections.Generic;
using System.Net;

using TrafficFilter.Configuration;
using TrafficFilter.Core;
using TrafficFilter.RequestFilters;

using Xunit;

namespace TrafficFilter.Tests
{
    public class TrafficFilterTests
    {
        [Fact]
        public void NotInBlacklistAndNoMatchReturnsAllowed()
        {
            //Arrange
            var ipAddress = IPAddress.Parse("192.168.0.1");

            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress= ipAddress
                }
            };

            var ipBlackList = Substitute.For<IIpBlacklist>();
            ipBlackList.IsInBlacklist(ipAddress).Returns(false);

            var requestFilter = Substitute.For<IRequestFilter>();
            requestFilter.IsMatch(httpContext).Returns(false);

            var requestFiltersFactory = Substitute.For<IRequestFiltersFactory>();
            requestFiltersFactory.RequestFilters.Returns(new List<IRequestFilter>() { requestFilter });

            var options = Substitute.For<IOptions<TrafficFilterOptions>>();
            options.Value.Returns(new TrafficFilterOptions()
            {
            });

            var logger = Substitute.For<ILogger<TrafficFilter>>();

            var trafficFilter = new TrafficFilter(ipBlackList, requestFiltersFactory, options, logger);

            //Act
            var isAllowed = trafficFilter.IsAllowed(httpContext);

            //Assert
            isAllowed.Should().BeTrue();
            trafficFilter.TrafficFilterOptions.Should().NotBeNull();
        }

        [Fact]
        public void InBlacklistReturnsNotAllowed()
        {
            //Arrange
            var ipAddress = IPAddress.Parse("192.168.0.1");

            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress= ipAddress
                }
            };

            var ipBlackList = Substitute.For<IIpBlacklist>();
            ipBlackList.IsInBlacklist(ipAddress).Returns(true);

            var requestFilter = Substitute.For<IRequestFilter>();
            requestFilter.IsMatch(httpContext).Returns(false);

            var requestFiltersFactory = Substitute.For<IRequestFiltersFactory>();
            requestFiltersFactory.RequestFilters.Returns(new List<IRequestFilter>() { requestFilter });

            var options = Substitute.For<IOptions<TrafficFilterOptions>>();
            options.Value.Returns(new TrafficFilterOptions()
            {
            });

            var logger = Substitute.For<ILogger<TrafficFilter>>();

            var trafficFilter = new TrafficFilter(ipBlackList, requestFiltersFactory, options, logger);

            //Act
            var isAllowed = trafficFilter.IsAllowed(httpContext);

            //Assert
            isAllowed.Should().BeFalse();
        }

        [Fact]
        public void NotInBlacklistAndMatchReturnsNotAllowed()
        {
            //Arrange
            var ipAddress = IPAddress.Parse("192.168.0.1");

            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress= ipAddress
                }
            };

            var ipBlackList = Substitute.For<IIpBlacklist>();
            ipBlackList.IsInBlacklist(ipAddress).Returns(false);

            var requestFilter = Substitute.For<IRequestFilter>();
            requestFilter.IsMatch(httpContext).Returns(true);

            var requestFiltersFactory = Substitute.For<IRequestFiltersFactory>();
            requestFiltersFactory.RequestFilters.Returns(new List<IRequestFilter>() { requestFilter });

            var options = Substitute.For<IOptions<TrafficFilterOptions>>();
            options.Value.Returns(new TrafficFilterOptions()
            {
            });

            var logger = Substitute.For<ILogger<TrafficFilter>>();

            var trafficFilter = new TrafficFilter(ipBlackList, requestFiltersFactory, options, logger);

            //Act
            var isAllowed = trafficFilter.IsAllowed(httpContext);

            //Assert
            isAllowed.Should().BeFalse();
        }
    }
}