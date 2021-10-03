using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Http;

namespace TrafficFilter.RequestFilters
{
    public interface IRequestFilter
    {
        bool IsEnabled { get; }

        bool IsMatch(HttpContext httpContext);

        int Order { get; }
    }

    public interface IRequestFiltersFactory
    {
        IList<IRequestFilter> RequestFilters { get; }
    }

    public class RequestFiltersFactory : IRequestFiltersFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private IList<IRequestFilter> _requestFilters;

        public RequestFiltersFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IList<IRequestFilter> RequestFilters
        {
            get
            {
                return _requestFilters ??= typeof(IRequestFilter).Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract)
                        .Where(t => typeof(IRequestFilter).IsAssignableFrom(t))
                        .Select(t => (IRequestFilter)_serviceProvider.GetService(t))
                        .OrderBy(f => f.Order)
                        .ToList();
            }
        }
    }
}