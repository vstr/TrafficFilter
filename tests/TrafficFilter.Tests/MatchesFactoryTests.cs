using FluentAssertions;

using TrafficFilter.Matches;

using Xunit;

namespace TrafficFilter.Tests
{
    public class MatchesFactoryTests
    {
        [Fact]
        public void MatchesFactoryProducesNonNullInstance()
        {
            //Arrange
            var matchesFactory = new MatchesFactory();

            //Act
            var matchContains = matchesFactory.GetInstance("Contains", "test");

            //Assert
            matchContains.Should().NotBeNull();
            matchContains.Match.Should().BeEquivalentTo("test");
        }
    }
}