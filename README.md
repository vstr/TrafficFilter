# ![TrafficFilter](https://raw.githubusercontent.com/vstr/TrafficFilter/main/assets/TrafficFilter64x64.png) TrafficFilter

ASP.NET Core middleware for request filtering and rate limiting. Configuration based URL Filter, Headers Filter and Rate Limiter.

# About

TrafficFilter is an ASP.NET Core middleware that enables request filtering and rate-limiting. Once any filtering rule matches, the requester's IP address is blacklisted for the duration of a configured period. The following request filters are available:
- URL
- Headers
- Rate Limiting

Each filter can be enabled and configured in the app config file.

TrafficFilter may be useful in scenarios when you want to protect your origin server resources from scanners/bots that try to access non-existent URLs.

Another use case could be protecting the app from accessing it using a public IP address.

TrafficFilter can also block requests from further processing if the configured rate limit is reached.

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
    "RequestFilterUrl": {
      "IsEnabled": true,
      "Matches": [
        {
          "Type": "Regex",
          "Match": "https?:\\/\\/[\\d*\\.*]+" //Pattern for IP Address based Url
        },
        {
          "Type": "EndsWith",
          "Match": ".xml"
        },
        {
          "Type": "Contains",
          "Match": "mysql"
        },
        {
          "Type": "StartsWith",
          "Match": "ftp"
        }
      ]
    },
    "RequestFilterHeaders": {
      "IsEnabled": true,
      "Matches": [
        {
          "Header": "user-agent",
          "Type": "Contains",
          "Match": "x-bot"
        }
      ]
    },
    "RateLimiterByPath": {
      "IsEnabled": true,
      "RateLimiterWindowSeconds": 2,
      "RateLimiterRequestLimit": 2,
      "WhitelistUrls": [
        {
          "Type": "EndsWith",
          "Match": ".mp4"
        }
      ]
    },
    "RateLimiterGlobal": {
      "IsEnabled": true,
      "RateLimiterWindowSeconds": 2,
      "RateLimiterRequestLimit": 2,
      "WhitelistUrls": [
        {
          "Type": "Contains",
          "Match": "upload"
        }
      ]
    }
  }
```

## Documentation

If any of the enabled filters matches the incoming request, the requester's IP address is added to the blacklist for the duration of `IPBlacklistTimeoutSeconds` and `HttpStatusCode.TooManyRequests` is returned.

Possible values for Match Type are: `StartsWith`, `Contains`, `EndsWith` and `Regex`.

Rate limiting for `RateLimiterByPath` is applied per IP address / HttpRequest.Path (ignoring query string)
Rate limiting for `RateLimiterGlobal` is applied per IP address / any request

To support Cloudflare setup, use `forwardedOptions.FillKnownNetworks()` extension method to load and populate known networks.

Take a look at `SampleWebApp` for configuration details if needed.

## Credits
Icons made by [Freepik](https://www.freepik.com) from [www.flaticon.com](https://www.flaticon.com/)

## License

[Apache 2.0](https://raw.githubusercontent.com/vstr/TrafficFilter/main/LICENSE)

