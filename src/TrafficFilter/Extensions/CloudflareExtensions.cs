using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

namespace TrafficFilter.Extensions
{
    public static class CloudflareExtensions
    {
        public static void FillKnownNetworks(this ForwardedHeadersOptions forwardedHeadersOptions)
        {
            foreach (var knownNetwork in GetIPNetworks())
            {
                var networkSplit = knownNetwork.Split('/');
                var ipNetwork = new IPNetwork(IPAddress.Parse(networkSplit[0]), int.Parse(networkSplit[1]));
                forwardedHeadersOptions.KnownNetworks.Add(ipNetwork);
            }
        }

        private static IList<string> GetIPNetworks()
        {
            var httpClient = new HttpClient();
            var taskIPv4 = httpClient.GetStringAsync("https://www.cloudflare.com/ips-v4");
            var taskIPv6 = httpClient.GetStringAsync("https://www.cloudflare.com/ips-v6");
            Task.WaitAll(taskIPv4, taskIPv6);
            var result = new List<string>();

            if (taskIPv4.IsCompletedSuccessfully && taskIPv6.IsCompletedSuccessfully
                && !string.IsNullOrEmpty(taskIPv4.Result) && !string.IsNullOrEmpty(taskIPv6.Result))
            {
                result.AddRange(taskIPv4.Result.Split("\n", StringSplitOptions.RemoveEmptyEntries));
                result.AddRange(taskIPv6.Result.Split("\n", StringSplitOptions.RemoveEmptyEntries));
            }

            return result;
        }
    }
}