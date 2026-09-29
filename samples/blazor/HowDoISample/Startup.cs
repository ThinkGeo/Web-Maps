using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ThinkGeo.UI.Blazor.HowDoI.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            // The Export to PDF page's raster print brings the map's picture back over the circuit;
            // the default limit on what the browser may send in one message is far below a screenshot.
            services.AddServerSideBlazor().AddHubOptions(options => options.MaximumReceiveMessageSize = 16 * 1024 * 1024);
            services.AddSingleton<MenuService>();
            services.AddSingleton<DemographicMapService>();
            services.AddSingleton<SourceCodeLoader>();
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                // A file a page has made - the PDF of Export to PDF, the picture of Draw the Map
                // on an Image - is fetched from here as any file is, by the token the page got.
                endpoints.MapGet("/export/{id}", context =>
                {
                    if (ExportStore.TryGet((string)context.Request.RouteValues["id"], out var bytes, out var contentType))
                    {
                        context.Response.ContentType = contentType;
                        return context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    }
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return System.Threading.Tasks.Task.CompletedTask;
                });
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
