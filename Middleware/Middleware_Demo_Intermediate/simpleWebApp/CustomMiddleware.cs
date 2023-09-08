using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace simpleWebApp
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IPrinter printer;

        public CustomMiddleware(RequestDelegate next, IPrinter printer)
        {
            this.next = next;
            this.printer = printer;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await httpContext.Response.WriteAsync("Inside the new custom middleware");
            await next(httpContext);
            printer.Print();
        }
    }

    public interface IPrinter
    {
        public void Print();
    }

    public class Printer : IPrinter
    {
        public void Print()
        {
            System.Console.WriteLine("\n Printer is printing... \n");
        }
    }
}