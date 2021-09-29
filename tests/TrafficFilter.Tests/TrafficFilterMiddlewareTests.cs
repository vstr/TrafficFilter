using System.Net;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

using TrafficFilter.Configuration;
using TrafficFilter.RateLimit;
using TrafficFilter.RequestFiltering;

using Xunit;

namespace TrafficFilter.Tests
{
    public class TrafficFilterMiddlewareTests
    {
        [Fact]
        public async Task IPAddressIsNotBlacklistedByDefault()
        {
            //Arrange
            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress = IPAddress.Parse("192.168.0.1")
                }
            };
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("192.168.0.1");
            httpContext.Request.Path = new PathString("/path");
            httpContext.Request.PathBase = new PathString("/pathbase");
            httpContext.Request.QueryString = new QueryString("?a=1");

            RequestDelegate next = (HttpContext context) => Task.CompletedTask;

            var trafficFilterMiddleware = new TrafficFilterMiddleware(next);

            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var ipBlacklist = Substitute.For<IpBlacklist>();
            var requestUrlFilter = Substitute.For<IRequestFilterUrl>();
            var requestHeadersFilter = Substitute.For<IRequestFilterHeaders>();
            var rateLimiter = Substitute.For<IRateLimiter>();
            var options = Substitute.For<IOptions<TrafficFilterOptions>>();
            options.Value.Returns(new TrafficFilterOptions()
            {
                IPBlacklistTimeoutSeconds = 1
            });

            //Act
            await trafficFilterMiddleware.InvokeAsync(httpContext, ipBlacklist, requestUrlFilter,
                requestHeadersFilter, rateLimiter, options, logger);

            //Assert
            httpContext.Response.Should().NotBeNull();
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task IPAddressIsAlreadyBlacklisted()
        {
            //Arrange
            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress = IPAddress.Parse("192.168.0.1")
                }
            };
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("192.168.0.1");
            httpContext.Request.Path = new PathString("/path");
            httpContext.Request.PathBase = new PathString("/pathbase");
            httpContext.Request.QueryString = new QueryString("?a=1");

            RequestDelegate next = (HttpContext context) => Task.CompletedTask;

            var trafficFilterMiddleware = new TrafficFilterMiddleware(next);

            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var ipBlacklist = Substitute.For<IIpBlacklist>();
            ipBlacklist.IsInBlacklist("192.168.0.1").Returns(true);
            var requestUrlFilter = Substitute.For<IRequestFilterUrl>();
            var requestHeadersFilter = Substitute.For<IRequestFilterHeaders>();
            var rateLimiter = Substitute.For<IRateLimiter>();
            var options = Substitute.For<IOptions<TrafficFilterOptions>>();
            options.Value.Returns(new TrafficFilterOptions()
            {
                IPBlacklistTimeoutSeconds = 1
            });

            //Act
            await trafficFilterMiddleware.InvokeAsync(httpContext, ipBlacklist, requestUrlFilter,
                requestHeadersFilter, rateLimiter, options, logger);

            //Assert
            httpContext.Response.Should().NotBeNull();
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.TooManyRequests);
        }

        [Fact]
        public async Task NullIPAddressIsBlacklisted()
        {
            //Arrange
            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    //RemoteIpAddress = IPAddress.Parse("192.168.0.1")
                }
            };
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("192.168.0.1");
            httpContext.Request.Path = new PathString("/path");
            httpContext.Request.PathBase = new PathString("/pathbase");
            httpContext.Request.QueryString = new QueryString("?a=1");

            RequestDelegate next = (HttpContext context) => Task.CompletedTask;

            var trafficFilterMiddleware = new TrafficFilterMiddleware(next);

            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var ipBlacklist = Substitute.For<IIpBlacklist>();
            var requestUrlFilter = Substitute.For<IRequestFilterUrl>();
            var requestHeadersFilter = Substitute.For<IRequestFilterHeaders>();
            var rateLimiter = Substitute.For<IRateLimiter>();
            var options = Substitute.For<IOptions<TrafficFilterOptions>>();
            options.Value.Returns(new TrafficFilterOptions()
            {
                IPBlacklistTimeoutSeconds = 1
            });

            //Act
            await trafficFilterMiddleware.InvokeAsync(httpContext, ipBlacklist, requestUrlFilter,
                requestHeadersFilter, rateLimiter, options, logger);

            //Assert
            httpContext.Response.Should().NotBeNull();
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.TooManyRequests);
        }

        [Fact]
        public async Task InvalidIPAddressIsBlacklisted()
        {
            //Arrange
            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress = IPAddress.Parse("192.168.0.1")
                }
            };
            httpContext.Request.Scheme = "https";
            //httpContext.Request.Host = new HostString("192.168.0.1");
            httpContext.Request.Path = new PathString("/path");
            httpContext.Request.PathBase = new PathString("/pathbase");
            httpContext.Request.QueryString = new QueryString("?a=1");

            RequestDelegate next = (HttpContext context) => Task.CompletedTask;

            var trafficFilterMiddleware = new TrafficFilterMiddleware(next);

            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var ipBlacklist = Substitute.For<IIpBlacklist>();
            var requestUrlFilter = Substitute.For<IRequestFilterUrl>();
            var requestHeadersFilter = Substitute.For<IRequestFilterHeaders>();
            var rateLimiter = Substitute.For<IRateLimiter>();
            var options = Substitute.For<IOptions<TrafficFilterOptions>>();
            options.Value.Returns(new TrafficFilterOptions()
            {
                IPBlacklistTimeoutSeconds = 1
            });

            //Act
            await trafficFilterMiddleware.InvokeAsync(httpContext, ipBlacklist, requestUrlFilter,
                requestHeadersFilter, rateLimiter, options, logger);

            //Assert
            httpContext.Response.Should().NotBeNull();
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.TooManyRequests);
        }

        [Fact]
        public async Task IPAddressIsBlacklistedOnRateLimit()
        {
            //Arrange
            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress = IPAddress.Parse("192.168.0.1")
                }
            };
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("192.168.0.1");
            httpContext.Request.Path = new PathString("/path");
            httpContext.Request.PathBase = new PathString("/pathbase");
            httpContext.Request.QueryString = new QueryString("?a=1");

            RequestDelegate next = (HttpContext context) => Task.CompletedTask;

            var trafficFilterMiddleware = new TrafficFilterMiddleware(next);

            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var ipBlacklist = Substitute.For<IIpBlacklist>();
            var requestUrlFilter = Substitute.For<IRequestFilterUrl>();
            var requestHeadersFilter = Substitute.For<IRequestFilterHeaders>();
            var rateLimiter = Substitute.For<IRateLimiter>();
            rateLimiter.IsLimitReached("192.168.0.1", "/path").Returns(true);
            var options = Substitute.For<IOptions<TrafficFilterOptions>>();
            options.Value.Returns(new TrafficFilterOptions()
            {
                IPBlacklistTimeoutSeconds = 1
            });

            //Act
            await trafficFilterMiddleware.InvokeAsync(httpContext, ipBlacklist, requestUrlFilter,
                requestHeadersFilter, rateLimiter, options, logger);

            //Assert
            httpContext.Response.Should().NotBeNull();
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.TooManyRequests);
        }
    }
}
