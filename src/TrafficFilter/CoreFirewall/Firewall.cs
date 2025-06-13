using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrafficFilter.CoreFirewall.Configuration;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;

namespace TrafficFilter.CoreFirewall
{
    public interface IFirewall
    {
        bool IsMatch(HttpContext httpContext);
    }

    public class Firewall : IFirewall
    {
        private readonly IMatchesFactory _matchesFactory;
        private readonly ILogger<Firewall> _logger;
        private readonly FirewallOptions _options;
        private readonly RulesContainer _rulesContainer;

        public Firewall(
            IMatchesFactory matchesFactory,
            IOptions<FirewallOptions> optionsAccessor,
            ILogger<Firewall> logger)
        {
            _logger = logger;

            _matchesFactory = matchesFactory ?? throw new ArgumentNullException(nameof(matchesFactory), "Matches factory cannot be null.");

            _options = optionsAccessor.Value;

            if (_options.IsEnabled)
            {
                _options.Validate();

                _rulesContainer = new RulesContainer(_matchesFactory, _options.BlockRules);
            }
        }

        public bool IsMatch(HttpContext httpContext)
        {
            if (!_options.IsEnabled)
            {
                return false;
            }

            var matchResult = _rulesContainer.IsMatch(httpContext);

            if (matchResult == null || matchResult.Count == 0)
            {
                return false;
            }

            foreach (var rule in matchResult)
            {
                _logger.LogInformation($"Firewall rule '{rule.GetType().Name}' matched '{rule.MatchType}' value '{rule.MatchValue}' for {httpContext.GetIPAddress()} rule group: {rule.Group} - request rejected {httpContext.GetDisplayUrl()} {httpContext.Request.Method}");
            }

            return true;
        }
    }
}