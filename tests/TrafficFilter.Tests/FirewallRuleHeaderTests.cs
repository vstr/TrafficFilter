using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using TrafficFilter.Configuration;
using TrafficFilter.CoreFirewall;
using TrafficFilter.CoreFirewall.Configuration;
using TrafficFilter.Matches;
using Xunit;

namespace TrafficFilter.Tests
{
    public class FirewallRuleHeaderTests
    {
        [Fact]
        public void FirewallRuleHeader_Exact()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "Exact",
                Match = "test-value",
                RequestPart = "Header:X-Test-Header",
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Headers.Returns(new HeaderDictionary()
            {
                  new KeyValuePair<string, StringValues>( "X-Test-Header", new StringValues("test-value"))
            });

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContext);

            Assert.True(isMatch, "Firewall rule should match the header value in the context.");
        }

        [Fact]
        public void FirewallRuleHeader_Regex()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "Regex",
                Match = @"test-\d+",
                RequestPart = "header:X-Test-Header"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Headers.Returns(new HeaderDictionary()
            {
                  new KeyValuePair<string, StringValues>( "X-Test-Header", new StringValues("test-123"))
            });

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContext);

            Assert.True(isMatch, "Firewall rule should match the header value in the context.");
        }

        [Fact]
        public void FirewallRuleHeader_Contains()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "Contains",
                Match = "value",
                RequestPart = "header:X-Test-Header"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Headers.Returns(new HeaderDictionary()
            {
                  new KeyValuePair<string, StringValues>( "X-Test-Header", new StringValues("test-value"))
            });

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContext);

            Assert.True(isMatch, "Firewall rule should match the header value in the context.");
        }

        [Fact]
        public void FirewallRuleHeader_StartsWith()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "StartsWith",
                Match = "test-",
                RequestPart = "header:X-Test-Header"
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Headers.Returns(new HeaderDictionary()
            {
                  new KeyValuePair<string, StringValues>( "X-Test-Header", new StringValues("test-value"))
            });

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContext);

            Assert.True(isMatch, "Firewall rule should match the header value in the context.");
        }

        [Fact]
        public void FirewallRuleHeader_EndsWith()
        {
            var ruleOptions = new RuleOptions
            {
                MatchType = "EndsWith",
                Match = "-value",
                RequestPart = "header:X-Test-Header",
            };
            var options = Options.Create(new FirewallOptions
            {
                IsEnabled = true,
                BlockRules = [ruleOptions]
            });

            var logger = Substitute.For<ILogger<Firewall>>();
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Headers.Returns(new HeaderDictionary()
            {
                  new KeyValuePair<string, StringValues>( "X-Test-Header", new StringValues("test-value"))
            });

            var matchesFactory = new MatchesFactory();

            var firewall = new Firewall(matchesFactory, options, logger);

            var isMatch = firewall.IsMatch(httpContext);

            Assert.True(isMatch, "Firewall rule should match the header value in the context.");
        }
    }
}