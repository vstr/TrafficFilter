# ![TrafficFilter](https://raw.githubusercontent.com/vstr/TrafficFilter/main/assets/TrafficFilter64x64.png) TrafficFilter

Tiny ASP.NET Core middleware for request filtering and rate limiting. Configuration based Firewall and RateLimeter.

# About

TrafficFilter is a lightweight ASP.NET Core middleware that enables request filtering and rate-limiting. Once any firewall rule or rate limiting matches, the requester's IP address is blacklisted for the duration of a configured period. The following rules are available:
- Request URL
- Request Header
- Request IP Address

Each rule can be configured in the appsettings.json file.

TrafficFilter may be useful in scenarios when you want to protect your origin server resources from unwanted scanners/bots.

TrafficFilter will block requests from further processing if the configured rule matched or requests rate limit is reached.

## Getting Started

First install the [TrafficFilter](https://www.nuget.org/packages/TrafficFilter/) NuGet package using PowerShell:
                                  
```powershell
PM> Install-Package TrafficFilter
```

or via the dotnet command line:

```
dotnet add package TrafficFilter
```

Then add the TrafficFilter middleware to your ASP.NET Core `Startup` class:

```csharp
using TrafficFilter;

namespace SampleWebApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // --- TrafficFilter ---
            services.AddTrafficFilter(Configuration);

            //...
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsProduction())
            {
                var forwardedOptions = new ForwardedHeadersOptions()
                {
                    ForwardedHeaders = ForwardedHeaders.All,
                    ForwardLimit = null
                };
                forwardedOptions.FillKnownNetworks(logger); // TrafficFilter extension to load Cloudflare IP ranges and fill KnownNetworks (https://www.cloudflare.com/ips/)
                app.UseForwardedHeaders(forwardedOptions);
            }

            // --- TrafficFilter ---
            app.UseTrafficFilter();

            //...
        }
    }
}
```

Add TrafficFilter configuration section to `appsettings.json`, modify it as needed:

```json
    "TrafficFilter": {
        "IPBlacklistTimeoutSeconds": 5,
        "Firewall": {
            "IsEnabled": true,
            "BlockRules": [
                {
                    "RequestPart": "Url", // Possible options: "Url|Header:user-agent|IP"
                    "MatchType": "EndsWith", // Possible options: "Regex|Contains|StartsWith|EndsWith|Exact|NullOrEmpty"
                    "Match": ".php"
                },
                {
                    "RequestPart": "IP",
                    "MatchType": "Regex",
                    "Match": "10\\.10\\.\\d+\\.\\d+"
                },
                {
                    "RequestPart": "Header:user-agent",
                    "MatchType": "EndsWith",
                    "Match": "BadBot"
                }
            ]
        },
        "RateLimiter": {
            "IsEnabled": true,
            "WindowSeconds": 2,
            "RequestsLimit": 20, // If there were 20 requests (RequestsLimit) from the same IP in the last 2 seconds (WindowSeconds), the IP will be blacklisted
            "WhitelistRules": [
                {
                    "RequestPart": "Url",
                    "MatchType": "EndsWith",
                    "Match": ".jpg"
                }
            ]
        }
    }
```

## Documentation

If any of the enabled rules matches the incoming request, the requester's IP address is added to the blacklist for the duration of `IPBlacklistTimeoutSeconds` and `HttpStatusCode.TooManyRequests` is returned.

Possible values for MatchType are: `Regex`, `Contains`, `StartsWith`, `EndsWith`, `Exact`, `NullOrEmpty`.

Possible values for RequestPart are: `Url`, `Header:header-name`, `IP`.

Rate limiting `RateLimiter` is applied per IP address

To support deployments bihind Cloudflare, use `forwardedOptions.LoadCloudflareKnownNetworks()` extension method to load and populate known networks.

Take a look at `SampleWebApp` for configuration details if needed.

## Credits
Icons made by [Freepik](https://www.freepik.com) from [www.flaticon.com](https://www.flaticon.com/)

## License

[Apache 2.0](https://raw.githubusercontent.com/vstr/TrafficFilter/main/LICENSE)

