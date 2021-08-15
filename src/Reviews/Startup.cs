using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Reviews
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
            services.AddSingleton(sp => new HttpClient(new TracedHttpMessageHandler()));
            services.AddSingleton(sp =>
            {
                var settings = new Settings
                {
                    StarColor = System.Environment.GetEnvironmentVariable("STAR_COLOR"),
                    RatingsEnabled = !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("ENABLE_RATINGS")),
                    ServiceDomain = System.Environment.GetEnvironmentVariable("SERVICES_DOMAIN"),
                    RatingsHostName = System.Environment.GetEnvironmentVariable("RATINGS_HOSTNAME"),
                    ExtraHeadersToForward = System.Environment.GetEnvironmentVariable("EXTRA_HEADERS_TO_FORWARD"),
                };

                if (string.IsNullOrEmpty(settings.StarColor))
                {
                    settings.StarColor = "black";
                }
                if (string.IsNullOrEmpty(settings.RatingsHostName))
                {
                    settings.RatingsHostName = "ratings";
                }
                if (string.IsNullOrEmpty(settings.ExtraHeadersToForward))
                {
                    settings.ExtraHeadersToForward = "";
                }

                return settings;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}