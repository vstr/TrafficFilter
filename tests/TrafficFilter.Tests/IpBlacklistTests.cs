using FluentAssertions;

using Xunit;

namespace TrafficFilter.Tests
{
    public class IpBlacklistTests
    {
        [Fact]
        public void Add_Contains_True()
        {
            //Arrange
            var ipBlackList = new IpBlacklist();

            //Act
            ipBlackList.Add("1", 10);

            //Assert
            ipBlackList.IsInBlacklist("1").Should().BeTrue();
            ipBlackList.Contains("1").Should().BeTrue();
        }

        [Fact]
        public void NoAdd_Contains_False()
        {
            //Arrange
            var ipBlackList = new IpBlacklist();

            //Act
            //Nop

            //Assert
            ipBlackList.IsInBlacklist("1").Should().BeFalse();
            ipBlackList.Contains("1").Should().BeFalse();
        }

        [Fact]
        public void Add_Different_Contains_False()
        {
            //Arrange
            var ipBlackList = new IpBlacklist();

            //Act
            ipBlackList.Add("2", 10);

            //Assert
            ipBlackList.IsInBlacklist("1").Should().BeFalse();
            ipBlackList.Contains("1").Should().BeFalse();
        }

        [Fact]
        public void Expired_Contains_False()
        {
            //Arrange
            var ipBlackList = new IpBlacklist();

            //Act
            ipBlackList.Add("1", 0);

            //Assert
            ipBlackList.IsInBlacklist("1").Should().BeFalse();
            ipBlackList.Contains("1").Should().BeFalse();
        }
    }
}
