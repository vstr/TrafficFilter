using System.Net;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using NSubstitute;

using TrafficFilter.Extensions;

using Xunit;

namespace TrafficFilter.Tests
{
    public class IPAddressHelperTests
    {
        [Fact]
        public void GetIPAddressShouldReturnNullOnNullContext()
        {
            //Arrange

            //Act
            var ipAddress = ((HttpContext)null).GetIPAddress();

            //Assert
            ipAddress.Should().BeNull();
        }

        [Fact]
        public void GetIPAddressForReverseProxyShouldReturnNullOnNullContext()
        {
            //Arrange

            //Act
            var ipAddress = ((HttpContext)null).GetIPAddress(true);

            //Assert
            ipAddress.Should().BeNull();
        }

        [Fact]
        public void GetIPAddressShouldReturnAddressIfRemoteAddressIsSet()
        {
            //Arrange
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(IPAddress.Parse("192.168.1.1"));
            contextAccessor.HttpContext.Request.Host.Returns(new HostString("192.168.1.1"));
            contextAccessor.HttpContext.Request.PathBase.Returns(new PathString("/basepath"));
            contextAccessor.HttpContext.Request.Path.Returns(new PathString("/path"));
            contextAccessor.HttpContext.Request.QueryString.Returns(new QueryString("?a=1"));
            contextAccessor.HttpContext.Request.Scheme.Returns("https");

            //Act
            var ipAddress = contextAccessor.HttpContext.GetIPAddress(false);

            //Assert
            ipAddress.Should().BeEquivalentTo(ipAddress);
        }

        [Fact]
        public void GetIPAddressShouldReturnAddressIfBehindReverseProxy()
        {
            //Arrange
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(IPAddress.Parse("192.168.1.1"));
            contextAccessor.HttpContext.Request.Host.Returns(new HostString("192.168.1.1"));
            contextAccessor.HttpContext.Request.PathBase.Returns(new PathString("/basepath"));
            contextAccessor.HttpContext.Request.Path.Returns(new PathString("/path"));
            contextAccessor.HttpContext.Request.QueryString.Returns(new QueryString("?a=1"));
            contextAccessor.HttpContext.Request.Scheme.Returns("https");

            contextAccessor.HttpContext.Request.Headers["X_FORWARDED_FOR"] = "192.168.0.1, 192.168.10.20";

            //Act
            var ipAddress = contextAccessor.HttpContext.GetIPAddress(true);

            //Assert
            ipAddress.Should().BeEquivalentTo(IPAddress.Parse("192.168.10.20"));
        }

        [Fact]
        public void GetIPAddressShouldFallbackToDefaultRemoteAddress()
        {
            //Arrange
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(IPAddress.Parse("192.168.1.1"));
            contextAccessor.HttpContext.Request.Host.Returns(new HostString("192.168.1.1"));
            contextAccessor.HttpContext.Request.PathBase.Returns(new PathString("/basepath"));
            contextAccessor.HttpContext.Request.Path.Returns(new PathString("/path"));
            contextAccessor.HttpContext.Request.QueryString.Returns(new QueryString("?a=1"));
            contextAccessor.HttpContext.Request.Scheme.Returns("https");

            contextAccessor.HttpContext.Request.Headers["X_FORWARDED_FOR"] = ",";

            //Act
            var ipAddress = contextAccessor.HttpContext.GetIPAddress(true);

            //Assert
            ipAddress.Should().BeEquivalentTo(IPAddress.Parse("192.168.1.1"));
        }

        [Fact]
        public void GetIPAddressShouldReturnAddressIfBehindReverseProxyCF()
        {
            //Arrange
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(IPAddress.Parse("192.168.1.1"));
            contextAccessor.HttpContext.Request.Host.Returns(new HostString("192.168.1.1"));
            contextAccessor.HttpContext.Request.PathBase.Returns(new PathString("/basepath"));
            contextAccessor.HttpContext.Request.Path.Returns(new PathString("/path"));
            contextAccessor.HttpContext.Request.QueryString.Returns(new QueryString("?a=1"));
            contextAccessor.HttpContext.Request.Scheme.Returns("https");

            contextAccessor.HttpContext.Request.Headers["CF-Connecting-IP"] = "192.168.10.20";

            //Act
            var ipAddress = contextAccessor.HttpContext.GetIPAddress(true);

            //Assert
            ipAddress.Should().BeEquivalentTo(IPAddress.Parse("192.168.10.20"));
        }

        [Fact]
        public void GetIPAddressShouldReturnDeafaultIfNoHeadersSet()
        {
            //Arrange
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(IPAddress.Parse("192.168.1.1"));
            contextAccessor.HttpContext.Request.Host.Returns(new HostString("192.168.1.1"));
            contextAccessor.HttpContext.Request.PathBase.Returns(new PathString("/basepath"));
            contextAccessor.HttpContext.Request.Path.Returns(new PathString("/path"));
            contextAccessor.HttpContext.Request.QueryString.Returns(new QueryString("?a=1"));
            contextAccessor.HttpContext.Request.Scheme.Returns("https");

            //Act
            var ipAddress = contextAccessor.HttpContext.GetIPAddress(true);

            //Assert
            ipAddress.Should().BeEquivalentTo(IPAddress.Parse("192.168.1.1"));
        }

        [Fact]
        public void GetSavedIPAddressFromHttpContextItems()
        {
            //Arrange
            var httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress = IPAddress.Parse("192.168.1.1")
                }
            };

            //Act
            _ = httpContext.GetIPAddress(false);
            var ipAddress = httpContext.GetIPAddress();

            //Assert
            ipAddress.Should().BeEquivalentTo(IPAddress.Parse("192.168.1.1"));
            httpContext.GetIPAddress().Should().BeEquivalentTo(IPAddress.Parse("192.168.1.1"));
        }

        [Fact]
        public void GetIPAddressFromEmptyHttpContextItems()
        {
            //Arrange
            var httpContext = new DefaultHttpContext()
            {
            };

            //Act
            var ipAddress = httpContext.GetIPAddress(false);

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