using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace simpleWebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ConsoleLoggerMiddleware>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // If the side branch isn't run, print the path info
            app.Use(async (ctx, next) =>
            {
                await ctx.Response.WriteAsync(
                    $"Use --> PathBase: \"{ctx.Request.PathBase}\" \t Path: \"{ctx.Request.Path}\" \n");
                await next();
            });

            /*** Map ***/
            app.Map("/favicon.ico", (app) => { });

            app.Map("/somePath", appBuilder =>
            {
                appBuilder.Run(async ctx =>
                {
                    await ctx.Response.WriteAsync($"The PathBase is {ctx.Request.PathBase}\n");
                    await ctx.Response.WriteAsync($"The Path is {ctx.Request.Path}\n");
                });
            });

            // Sub-branching
            app.Map("/root", appBuilder =>
            {
                appBuilder.Map("/child1", childAppBuilder =>
                {
                    childAppBuilder.Run(
                    async ctx =>
                    {
                        await ctx.Response.WriteAsync($"The PathBase is {ctx.Request.PathBase}\n");
                        await ctx.Response.WriteAsync($"The Path is {ctx.Request.Path}\n");
                    });
                });
                appBuilder.Map("/child2", childAppBuilder =>
                {
                    childAppBuilder.Run(
                    async ctx =>
                    {
                        await ctx.Response.WriteAsync($"The PathBase is {ctx.Request.PathBase}\n");
                        await ctx.Response.WriteAsync($"The Path is {ctx.Request.Path}\n");
                    });
                });
            });

             app.MapWhen(ctx => ctx.Request.Query.ContainsKey("count"), appBuilder =>
                        {
                            appBuilder.Run(async context =>
                            {
                                await context.Response.WriteAsync($"MapWhen --> count is {context.Request.Query["count"]}");
                            });
                        });

            app.Map("/map", MapHandler);

            /** Custom Middleware **/
            app.UseMiddleware<ConsoleLoggerMiddleware>();

            /*** UseWhen ***/
            app.UseWhen(context => context.Request.Query.ContainsKey("q"), HandleRequestWithQuery);
            app.UseWhen(ctx => ctx.Request.Query.ContainsKey("role"), appBuilder =>
                        {
                            appBuilder.Run(async context =>
                            {
                                await context.Response.WriteAsync($"<br> Role is {context.Request.Query["role"]}");
                            });
                        });

            /*** Use ***/
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("<html><body>Response from First milddleware <br>");
                await next(); // subsequent middleware code will execute here (such as app.Run()) 
                await context.Response.WriteAsync("<br> End of Response from First milddleware <br></body></html>");
            });

            /*** Run ***/
            app.Run(async context =>
            {
                Console.WriteLine("Hello World");
                System.Console.WriteLine(context.Request.QueryString);
                await context.Response.WriteAsync("Hello World");
            });

        }

        private void HandleRequestWithQuery(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                Console.WriteLine("Contains Query");
                await next();
            });
        }

        private void MapHandler(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                Console.WriteLine("Hello for Map Method");
                await context.Response.WriteAsync("Hello from Map Method");
            });
        }
    }
}
