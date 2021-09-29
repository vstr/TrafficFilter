using System.Net;

using Microsoft.AspNetCore.Http;

using NSubstitute;

namespace TrafficFilter.Tests.Common
{
    public static class TestHelper
    {
        public static IHttpContextAccessor BuildHttpContextAccessor(string scheme, string ipAddress, string path = "/path", string queryString = "?a=1")
        {
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(IPAddress.Parse(ipAddress));
            contextAccessor.HttpContext.Request.Host.Returns(new HostString(ipAddress));
            contextAccessor.HttpContext.Request.PathBase.Returns(new PathString("/basepath"));
            contextAccessor.HttpContext.Request.Path.Returns(new PathString(path));
            contextAccessor.HttpContext.Request.QueryString.Returns(new QueryString(queryString));
            contextAccessor.HttpContext.Request.Scheme.Returns(scheme);
            return contextAccessor;
        }
    }
}
