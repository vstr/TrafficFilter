using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TrafficFilter;
using TrafficFilter.Extensions;

namespace SampleWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // --- TrafficFilter ---
            services.AddTrafficFilter(Configuration);

            services.AddControllersWithViews();

            services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
            });
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

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}