using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsReaderSPA.Provider;
using NewsReaderSPA.WebSocketConf;

namespace NewsReaderSPA
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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Todo: URL is hard coded it should be be placed in app
            //var NewsClient = new NewsClient("http://nu.nl/rss/Algemeen", 2);

            // Todo: Test URL must be moved to appsetting.Development.json
            // we will have control over the test URL to add new items and test the background
            // service.
            var NewsClient = new NewsClient("https://tmp.hashpanel.com/feed/", 10);
            NewsClient.Start();

            // Add WebSockets to project
            app.UseWebSockets(wsConfig.GetOptions());

            // WebSocket routings
            app.Use(async (context, next) =>
            {
                // If this is request to initial WebSocket connection
                if (context.WebSockets.IsWebSocketRequest)
                {

                    // Todo : Provisiniong of context must be done in WebSocketController
                    // Match the context URL on WebSocket 
                    // This is similar to routing with Web Http Request
                    // Todo : Context Path must be in appsettings.json
                    if (context.Request.Path == "/feed")
                    {

                        WebSocketController wsc = new WebSocketController(NewsClient);
                        await wsc.ListenAcceptAsync(context);

                    }
                }
                else
                {
                    // Else if request is not connection establishment
                    await next();

                }
            });

            // set Index.html as default page
            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("Index.html");
            app.UseDefaultFiles(options);
            app.UseStaticFiles();
        }
    }
}
