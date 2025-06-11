using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrafficFilter.CoreFirewall.Configuration;
using TrafficFilter.Extensions;
using TrafficFilter.Matches;
using TrafficFilter.Rules;

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
        private readonly IList<RuleBase> _blockRules;

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

                _blockRules = _options.BlockRules != null
                    ? _options.BlockRules.Select(r => RuleBase.BuildRule(_matchesFactory, r)).ToList()
                    : new List<RuleBase>();
            }
        }

        public bool IsMatch(HttpContext httpContext)
        {
            if (!_options.IsEnabled)
            {
                return false;
            }

            foreach (var rule in _blockRules)
            {
                if (rule.IsMatch(httpContext))
                {
                    _logger.LogInformation($"Firewall rule '{rule.GetType().Name}' matched '{rule.MatchType}' value '{rule.MatchValue}' for {httpContext.GetIPAddress()} - request rejected {httpContext.GetDisplayUrl()} {httpContext.Request.Method}");
                    return true;
                }
            }

            return false;
        }
    }
}