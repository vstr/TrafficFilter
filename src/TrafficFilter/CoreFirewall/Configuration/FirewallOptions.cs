using System;
using System.Collections.Generic;
using TrafficFilter.Configuration;

namespace TrafficFilter.CoreFirewall.Configuration
{
    public class FirewallOptions
    {
        public const string SectionName = "Firewall";
        public bool IsEnabled { get; set; }
        public List<RuleOptions> BlockRules { get; set; } = new List<RuleOptions>();

        public void Validate()
        {
            if (IsEnabled && (BlockRules == null || BlockRules.Count == 0))
            {
                throw new ArgumentException("At least one block rule must be defined.", nameof(BlockRules));
            }

            foreach (var rule in BlockRules)
            {
                rule.Validate();
            }
        }
    }
}