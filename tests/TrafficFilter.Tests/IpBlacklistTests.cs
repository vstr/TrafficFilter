using System.Net;

using FluentAssertions;

using TrafficFilter.Core;

using Xunit;

namespace TrafficFilter.Tests
{
    public class IpBlacklistTests
    {
        [Fact]
        public void NullIpAddressReturnsIsInBlacklistTrue()
        {
            //Arrange
            var ipBlackList = new IpBlacklist();

            //Act
            ipBlackList.Add(null, 10);

            //Assert
            ipBlackList.Contains(null).Should().BeFalse();
            ipBlackList.IsInBlacklist(null).Should().BeTrue();
        }

        [Fact]
        public void Add_Contains_True()
        {
            //Arrange
            var ipBlackList = new IpBlacklist();
            var ipAddress = IPAddress.Parse("192.168.0.1");

            //Act
            ipBlackList.Add(ipAddress, 10);

            //Assert
            ipBlackList.IsInBlacklist(ipAddress).Should().BeTrue();
            ipBlackList.Contains(ipAddress).Should().BeTrue();
        }

        [Fact]
        public void NoAdd_Contains_False()
        {
            //Arrange
            var ipBlackList = new IpBlacklist();
            var ipAddress = IPAddress.Parse("192.168.0.1");

            //Act
            //Nop

            //Assert
            ipBlackList.IsInBlacklist(ipAddress).Should().BeFalse();
            ipBlackList.Contains(ipAddress).Should().BeFalse();
        }

        [Fact]
        public void Add_Different_Contains_False()
        {
            //Arrange
            var ipBlackList = new IpBlacklist();
            var ipAddress1 = IPAddress.Parse("192.168.0.1");
            var ipAddress2 = IPAddress.Parse("192.168.0.2");

            //Act
            ipBlackList.Add(ipAddress2, 10);

            //Assert
            ipBlackList.IsInBlacklist(ipAddress1).Should().BeFalse();
            ipBlackList.Contains(ipAddress1).Should().BeFalse();
        }

        [Fact]
        public void Expired_Contains_False()
        {
            //Arrange
            var ipBlackList = new IpBlacklist();
            var ipAddress = IPAddress.Parse("192.168.0.1");

            //Act
            ipBlackList.Add(ipAddress, 0);

            //Assert
            ipBlackList.IsInBlacklist(ipAddress).Should().BeFalse();
            ipBlackList.Contains(ipAddress).Should().BeFalse();
        }
    }
}