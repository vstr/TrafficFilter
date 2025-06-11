using System.Collections.Generic;
using TrafficFilter.Configuration;

namespace TrafficFilter.CoreFirewall.Configuration
{
    public class FirewallOptions
    {
        public const string Firewall = nameof(Firewall);

        public bool IsEnabled { get; set; }
        public List<RuleOptions> BlockRules { get; set; }
    }
}