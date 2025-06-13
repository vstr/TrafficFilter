using System;
using NSubstitute;
using TrafficFilter.Configuration;
using TrafficFilter.Matches;
using TrafficFilter.Rules;
using Xunit;

namespace TrafficFilter.Tests
{
    public class RuleBaseTests
    {
        [Fact]
        public void BuildRule_Header_ReturnsRuleHeader()
        {
            var match = Substitute.For<IMatch>();
            var factory = Substitute.For<IMatchesFactory>();

            var ruleOptions = new RuleOptions
            {
                RequestPart = "header:X-Test",
                MatchType = "type",
                Match = "match",
                Group = "TestGroup"
            };

            factory.GetInstance(ruleOptions).Returns(match);
            var options = new RuleOptions
            {
                RequestPart = "header:X-Test",
                MatchType = "type",
                Match = "match"
            };

            var rule = RuleBase.BuildRule(factory, options);

            Assert.IsType<RuleHeader>(rule);
        }

        [Fact]
        public void BuildRule_Header_WithoutName_Throws()
        {
            var match = Substitute.For<IMatch>();
            var factory = Substitute.For<IMatchesFactory>();

            var ruleOptions = new RuleOptions
            {
                RequestPart = "header",
                MatchType = "type",
                Match = "match",
                Group = "TestGroup"
            };

            factory.GetInstance(ruleOptions).Returns(match);
            var options = new RuleOptions
            {
                RequestPart = "header",
                MatchType = "type",
                Match = "match"
            };

            Assert.Throws<ArgumentException>(() => RuleBase.BuildRule(factory, options));
        }

        [Fact]
        public void BuildRule_Ip_ReturnsRuleIp()
        {
            var match = Substitute.For<IMatch>();
            var factory = Substitute.For<IMatchesFactory>();

            var ruleOptions = new RuleOptions
            {
                RequestPart = "ip",
                MatchType = "type",
                Match = "match",
                Group = "TestGroup"
            };

            factory.GetInstance(ruleOptions).Returns(match);
            var options = new RuleOptions
            {
                RequestPart = "ip",
                MatchType = "type",
                Match = "match"
            };

            var rule = RuleBase.BuildRule(factory, options);

            Assert.IsType<RuleIp>(rule);
        }

        [Fact]
        public void BuildRule_Url_ReturnsRuleUrl()
        {
            var match = Substitute.For<IMatch>();
            var factory = Substitute.For<IMatchesFactory>();

            var ruleOptions = new RuleOptions
            {
                RequestPart = "url",
                MatchType = "type",
                Match = "match",
                Group = "TestGroup"
            };

            factory.GetInstance(ruleOptions).Returns(match);
            var options = new RuleOptions
            {
                RequestPart = "url",
                MatchType = "type",
                Match = "match"
            };

            var rule = RuleBase.BuildRule(factory, options);

            Assert.IsType<RuleUrl>(rule);
        }

        [Fact]
        public void BuildRule_UnsupportedRequestPart_Throws()
        {
            var match = Substitute.For<IMatch>();
            var factory = Substitute.For<IMatchesFactory>();

            var ruleOptions = new RuleOptions
            {
                RequestPart = "cookie:session",
                MatchType = "type",
                Match = "match",
                Group = "TestGroup"
            };

            factory.GetInstance(ruleOptions).Returns(match);
            var options = new RuleOptions
            {
                RequestPart = "cookie:session",
                MatchType = "type",
                Match = "match"
            };

            Assert.Throws<NotSupportedException>(() => RuleBase.BuildRule(factory, options));
        }
    }
}