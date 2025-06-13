using FluentAssertions;
using TrafficFilter.Configuration;
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

            var ruleOptions = new RuleOptions
            {
                RequestPart = "Url",
                MatchType = "Contains",
                Match = "test",
                Group = "TestGroup"
            };

            //Act
            var matchContains = matchesFactory.GetInstance(ruleOptions);

            //Assert
            matchContains.Should().NotBeNull();
            matchContains.Match.Should().BeEquivalentTo("test");
        }
    }
}