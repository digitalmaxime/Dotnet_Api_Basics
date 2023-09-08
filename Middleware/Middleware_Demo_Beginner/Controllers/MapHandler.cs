
public static class MapHandler
{
    public async static void Handle(HttpContext context)
    {
        System.Console.WriteLine("hello from MapHandler Method");
        await context.Response.WriteAsync("Hello from MapHandler Method");
    }
}