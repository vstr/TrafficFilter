using System;
using Microsoft.AspNetCore.Http;
using TrafficFilter.Matches;

namespace TrafficFilter.Rules
{
    public class RuleHeader : RuleBase
    {
        private readonly string _headerName;

        public RuleHeader(IMatch match, string headerName)
            : base(match)
        {
            if (string.IsNullOrWhiteSpace(headerName))
            {
                throw new ArgumentException("Header name cannot be empty or whitespace.", nameof(headerName));
            }

            _headerName = headerName;
        }

        protected override string GetRequestPartValue(HttpContext httpContext)
        {
            if (httpContext.Request.Headers == null
                || httpContext.Request.Headers.Count == 0
                || !httpContext.Request.Headers.TryGetValue(_headerName, out var value))
            {
                return string.Empty;
            }

            return value.ToString();
        }
    }
}