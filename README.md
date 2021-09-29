# TrafficFilter
(draft)

ASP.NET Core middleware for request filtering and rate limiting

# About

TrafficFilter is an ASP.NET Core middleware that enables request filtering and rate-limiting. There are the following request filtering features:
- Url filtering
- Headers filtering
- Rate limiting

Each feature can be enabled and configured in the config file. 

TrafficFilter may be useful in scenarios when you want to protect your web app and server resources from various scanning bots that try to access non-existent URLs by blacklisting their IP addresses for a configured amount of time.

Another use case could be protecting the app if it is accessed using a public server IP address. 

It can also block traffic if configured path-based rate limit is reached.

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // --- TrafficFilter - topmost important! ---
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
          "Type": "MatchRegex",
          "Match": "https?:\\/\\/[\\d*\\.*]+" //Pattern for IP Address based Url
        },
        {
          "Type": "MatchEndsWith",
          "Match": ".xml"
        },       
        {
          "Type": "MatchContains",
          "Match": "mysql"
        },
        {
          "Type": "MatchStartsWith",
          "Match": "ftp"
        }
      ]
    },
    "RequestFilterHeaders": {
      "IsEnabled": true,
      "Matches": [
        {
          "Header": "user-agent",
          "Type": "MatchContains",
          "Match": "x-bot"
        }
      ]
    },
    "RateLimiter": {
      "IsEnabled": true,
      "RateLimiterWindowSeconds": 1,
      "RateLimiterRequestLimit": 10,
      "SkipUrls": [ // Add matches here if you want to exclude them from rate limiting
        {
          "Type": "MatchEndsWith",
          "Match": ".mp4"
        }
      ]
    }
  }
```

## Documentation

If any of the enabled filters matches the incoming request, the requester's IP address is added to the blacklist for the duration of `IPBlacklistTimeoutSeconds` and `HttpStatusCode.TooManyRequests` is returned.

Possible values for Match Type are: `MatchStartsWith`, `MatchContains`, `MatchEndsWith` and `MatchRegex`.

## License

[Apache 2.0](https://raw.githubusercontent.com/vstr/TrafficFilter/main/LICENSE)


