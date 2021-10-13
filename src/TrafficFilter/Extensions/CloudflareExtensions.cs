using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;

namespace TrafficFilter.Extensions
{
    public static class CloudflareExtensions
    {
        public static void FillKnownNetworks(this ForwardedHeadersOptions forwardedHeadersOptions, ILogger logger = null)
        {
            logger?.LogInformation($"Filling Cloudflare's KnownNetworks");
            foreach (var knownNetwork in GetIPNetworks(logger))
            {
                var networkSplit = knownNetwork.Split('/');
                var ipNetwork = new IPNetwork(IPAddress.Parse(networkSplit[0]), int.Parse(networkSplit[1]));
                forwardedHeadersOptions.KnownNetworks.Add(ipNetwork);
                logger?.LogInformation($"Added KnownNetwork {knownNetwork}");
            }
        }

        private static IList<string> GetIPNetworks(ILogger logger)
        {
            var httpClient = new HttpClient();
            var ipV4Networks = GetIPNetworksAsync(httpClient, "https://www.cloudflare.com/ips-v4");
            var ipV6Networks = GetIPNetworksAsync(httpClient, "https://www.cloudflare.com/ips-v6");

            var result = new List<string>();

            try
            {
                Task.WaitAll(ipV4Networks, ipV6Networks);
                result.AddRange(ipV4Networks.Result);
                result.AddRange(ipV6Networks.Result);
            }
            catch (AggregateException ae)
            {
                foreach (var innerException in ae.Flatten().InnerExceptions)
                {
                    logger?.LogError(innerException, "Error loading IPNetworks");
                }
            }

            return result;
        }

        private static async Task<IList<string>> GetIPNetworksAsync(HttpClient httpClient, string url)
        {
            var ipNetworks = await httpClient.GetStringAsync(url);

            var result = new List<string>();

            if (!string.IsNullOrEmpty(ipNetworks))
            {
                result.AddRange(ipNetworks.Split("\n", StringSplitOptions.RemoveEmptyEntries));
            }

            return result;
        }
    }
}