using System.Net;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using NSubstitute;

using TrafficFilter.Configuration;

using Xunit;

namespace TrafficFilter.Tests
{
    public class IPAddressHelperTests
    {
        [Fact]
        public void GetIPAddressShouldReturnNullOnNullContext()
        {
            //Arrange
            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var options = new TrafficFilterOptions()
            {
            };

            //Act
            var ipAddress = ((HttpContext)null).GetIPAddress(options, logger);

            //Assert
            ipAddress.Should().BeNull();
        }

        [Fact]
        public void GetIPAddressShouldReturnAddressIfRemoteAddressIsSet()
        {
            //Arrange
            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var options = new TrafficFilterOptions()
            {
            };

            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(IPAddress.Parse("192.168.1.1"));
            contextAccessor.HttpContext.Request.Host.Returns(new HostString("192.168.1.1"));
            contextAccessor.HttpContext.Request.PathBase.Returns(new PathString("/basepath"));
            contextAccessor.HttpContext.Request.Path.Returns(new PathString("/path"));
            contextAccessor.HttpContext.Request.QueryString.Returns(new QueryString("?a=1"));
            contextAccessor.HttpContext.Request.Scheme.Returns("https");

            //Act
            var ipAddress = contextAccessor.HttpContext.GetIPAddress(options, logger);

            //Assert
            ipAddress.Should().BeEquivalentTo("192.168.1.1");
        }

        [Fact]
        public void GetIPAddressShouldReturnAddressIfBehindReverseProxy()
        {
            //Arrange
            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var options = new TrafficFilterOptions()
            {
                IsBehindReverseProxy = true
            };

            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(IPAddress.Parse("192.168.1.1"));
            contextAccessor.HttpContext.Request.Host.Returns(new HostString("192.168.1.1"));
            contextAccessor.HttpContext.Request.PathBase.Returns(new PathString("/basepath"));
            contextAccessor.HttpContext.Request.Path.Returns(new PathString("/path"));
            contextAccessor.HttpContext.Request.QueryString.Returns(new QueryString("?a=1"));
            contextAccessor.HttpContext.Request.Scheme.Returns("https");

            contextAccessor.HttpContext.Request.Headers["X_FORWARDED_FOR"] = "192.168.0.1, 192.168.10.20";

            //Act
            var ipAddress = contextAccessor.HttpContext.GetIPAddress(options, logger);

            //Assert
            ipAddress.Should().BeEquivalentTo("192.168.10.20");
        }

        [Fact]
        public void GetIPAddressShouldReturnAddressIfBehindReverseProxyCF()
        {
            //Arrange
            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var options = new TrafficFilterOptions()
            {
                IsBehindReverseProxy = true
            };

            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(IPAddress.Parse("192.168.1.1"));
            contextAccessor.HttpContext.Request.Host.Returns(new HostString("192.168.1.1"));
            contextAccessor.HttpContext.Request.PathBase.Returns(new PathString("/basepath"));
            contextAccessor.HttpContext.Request.Path.Returns(new PathString("/path"));
            contextAccessor.HttpContext.Request.QueryString.Returns(new QueryString("?a=1"));
            contextAccessor.HttpContext.Request.Scheme.Returns("https");

            contextAccessor.HttpContext.Request.Headers["CF-Connecting-IP"] = "192.168.10.20";

            //Act
            var ipAddress = contextAccessor.HttpContext.GetIPAddress(options, logger);

            //Assert
            ipAddress.Should().BeEquivalentTo("192.168.10.20");
        }

        [Fact]
        public void GetIPAddressShouldReturnDeafaultIfNoHeadersSet()
        {
            //Arrange
            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var options = new TrafficFilterOptions()
            {
                IsBehindReverseProxy = true
            };

            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(IPAddress.Parse("192.168.1.1"));
            contextAccessor.HttpContext.Request.Host.Returns(new HostString("192.168.1.1"));
            contextAccessor.HttpContext.Request.PathBase.Returns(new PathString("/basepath"));
            contextAccessor.HttpContext.Request.Path.Returns(new PathString("/path"));
            contextAccessor.HttpContext.Request.QueryString.Returns(new QueryString("?a=1"));
            contextAccessor.HttpContext.Request.Scheme.Returns("https");

            //Act
            var ipAddress = contextAccessor.HttpContext.GetIPAddress(options, logger);

            //Assert
            ipAddress.Should().BeEquivalentTo("192.168.1.1");
        }

        [Fact]
        public void GetSavedIPAddressFromHttpContextItems()
        {
            //Arrange
            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var options = new TrafficFilterOptions()
            {
            };

            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress = IPAddress.Parse("192.168.1.1")
                }
            };

            //Act
            var ipAddress = httpContext.GetIPAddress(options, logger);

            //Assert
            ipAddress.Should().BeEquivalentTo("192.168.1.1");
            httpContext.GetIPAddress().Should().BeEquivalentTo("192.168.1.1");
        }

        [Fact]
        public void GetIPAddressFromEmptyHttpContextItems()
        {
            //Arrange
            var logger = Substitute.For<ILogger<TrafficFilterMiddleware>>();
            var options = new TrafficFilterOptions()
            {
            };

            var httpContext = new DefaultHttpContext()
            {
            };

            //Act
            var ipAddress = httpContext.GetIPAddress(options, logger);

            //Assert
            ipAddress.Should().BeNull();
            httpContext.GetIPAddress().Should().BeNull();
        }

        [Fact]
        public void GetIPAddressOnNullHttpContext()
        {
            ((HttpContext)null).GetIPAddress().Should().BeNull();
        }

        [Fact]
        public void GetIPAddressOnEmptyHttpContextItems()
        {
            _ = new DefaultHttpContext().GetIPAddress().Should().BeNull();
        }
    }
}