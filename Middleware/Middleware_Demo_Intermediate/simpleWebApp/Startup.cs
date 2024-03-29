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
            services.AddSingleton<IPrinter, Printer>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExtensions();

            /** Custom Middleware **/
            app.UseMiddleware<ConsoleLoggerMiddleware>(); // implements : IMiddleware
            app.UseMiddleware<CustomMiddleware>(); // does not implements : IMiddleware

            app.Run(async context => await context.Response.WriteAsync("--- END ---"));
        }
    }
}
