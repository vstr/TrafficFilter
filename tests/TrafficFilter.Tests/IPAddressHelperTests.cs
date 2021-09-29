using System.Net;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using NSubstitute;

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
            var ipAddress = contextAccessor.HttpContext.GetIPAddress();

            //Assert
            ipAddress.Should().BeEquivalentTo("192.168.1.1");
        }
    }
}
