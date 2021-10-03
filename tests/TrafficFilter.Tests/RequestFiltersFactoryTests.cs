using System;

using FluentAssertions;

using NSubstitute;

using TrafficFilter.RequestFilters;

using Xunit;

namespace TrafficFilter.Tests
{
    public class RequestFiltersFactoryTests
    {
        [Fact]
        public void RequestFiltersFactoryShouldReturnRequestFilters()
        {
            //Arrange
            var requestFilter = Substitute.For<IRequestFilter>();

            var serviceProvider = Substitute.For<IServiceProvider>();
            _ = serviceProvider.GetService(typeof(RequestFilterUrl)).Returns(requestFilter);
            _ = serviceProvider.GetService(typeof(RequestFilterHeaders)).Returns(requestFilter);
            _ = serviceProvider.GetService(typeof(RequestFilterRateLimiter)).Returns(requestFilter);
            var factory = new RequestFiltersFactory(serviceProvider);

            //Act
            var requestFilters = factory.RequestFilters;

            //Assert
            requestFilters.Should().NotBeNull();
            requestFilters.Count.Should().Be(3);
        }
    }
}