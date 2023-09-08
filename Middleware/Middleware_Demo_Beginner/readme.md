<h2>Project Creation</h2>

`dotnet new web`

spins a basic web app without controller

---
<h2>3 main methods</h2>

- app.Run
- app.Use
- app.Map

---
<h3>Run<></h3>

`Run` : `app.Run(context =>`

Run is a convention naming to indicate the end the the Request pipeline.

Internally, it resolves to :
```
app.Use(_ => handler)
```



---
<h3>Use and UseMiddleware<></h3>

`Use` : `app.Use(async (ctx, next) =>`

You can declare classes which implement IMiddelware.
Those classes can be used in the API pipeline through  :

`app.UseMiddleware<MyMiddlewareClass>();`

code example : 

```
public class ConsoleLoggerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
```

In order to be able to use this class, it needs to be registered to ASPNET Core ServicesCollection

In the program style config:
`builder.Services.AddTransient<ConsoleLoggerMiddleware>();`

Or in the Startup style config:
```
public class Startup
	{
		private readonly IConfiguration _configuration;
		private readonly IHostEnvironment _environment;

		public Startup(IConfiguration configuration, IHostEnvironment environment)
		{
			_configuration = configuration;
			_environment = environment;
		}

        public void ConfigureServices(IServiceCollection services) {
            services.AddTransient<ConsoleLoggerMiddleware>();
        }
}
```

---
<h3>Map<></h3>

The`Map`method ends the middleware pipeline after its execution, unlike `Use`.
Anything after that won't be invoked.




