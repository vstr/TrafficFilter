using System;

namespace TrafficFilter.Configuration
{
    public class RuleOptions
    {
        public string RequestPart { get; set; }
        public string MatchType { get; set; }
        public string Match { get; set; }
        public string Group { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(RequestPart))
            {
                throw new ArgumentException($"RequestPart cannot be null or empty. {this}", nameof(RequestPart));
            }
            if (string.IsNullOrWhiteSpace(MatchType))
            {
                throw new ArgumentException($"MatchType cannot be null or empty. {this}", nameof(MatchType));
            }
            if (string.IsNullOrWhiteSpace(Match))
            {
                throw new ArgumentException($"Match cannot be null or empty. {this}", nameof(Match));
            }
        }

        public override string ToString()
        {
            return $"Rule {nameof(RequestPart)}:'{RequestPart}' {nameof(MatchType)}:'{MatchType}' {nameof(Match)}:'{Match}'";
        }
    }
}