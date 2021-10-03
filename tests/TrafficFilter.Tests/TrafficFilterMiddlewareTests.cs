using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using NSubstitute;

using TrafficFilter.Configuration;

using Xunit;

namespace TrafficFilter.Tests
{
    public class TrafficFilterMiddlewareTests
    {
        [Fact]
        public async Task TrafficFilterIsAllowedTrue()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();

            RequestDelegate next = (HttpContext context) => Task.CompletedTask;

            var trafficFilterMiddleware = new TrafficFilterMiddleware(next);

            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var trafficFilter = Substitute.For<ITrafficFilter>();
            trafficFilter.TrafficFilterOptions.Returns(new TrafficFilterOptions
            {
                IsBehindReverseProxy = true,
                IPBlacklistTimeoutSeconds = 10
            });
            trafficFilter.IsAllowed(httpContext).Returns(true);

            //Act
            await trafficFilterMiddleware.InvokeAsync(httpContext, trafficFilter);

            //Assert
            httpContext.Response.Should().NotBeNull();
            httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task TrafficFilterIsAllowedFalse()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();

            RequestDelegate next = (HttpContext context) => Task.CompletedTask;

            var trafficFilterMiddleware = new TrafficFilterMiddleware(next);

            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var trafficFilter = Substitute.For<ITrafficFilter>();
            trafficFilter.TrafficFilterOptions.Returns(new TrafficFilterOptions
            {
                IsBehindReverseProxy = true,
                IPBlacklistTimeoutSeconds = 10
            });
            trafficFilter.IsAllowed(httpContext).Returns(false);

            //Act
            await trafficFilterMiddleware.InvokeAsync(httpContext, trafficFilter);

            //Assert
            httpContext.Response.Should().NotBeNull();
            httpContext.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
        }
    }
}