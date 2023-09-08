<h2>3 Main methods </h2>

<h3>app.Run</h3>

```
app.Run(async (context) => {
    await context.Response.WriteAsync("Hello");
})
```
Nothing is executed beyond this point

<h3>app.Use()</h3>
```
app.Use(async (context, next) => {
    await context.Response.WriteAsync("<html><body>Response from First mildleware</body></html>");
    await next();
});
```

Without the use of next(), the 'Use' middleware will behave exactly like the 'Run' middleware

<h5>app.UseWhen()</h5>
Conditionally creates a branch in the request pipeline that **returns to the main pipeline.**

```
app.UseWhen(ctx => ctx.Request.Query.ContainsKey("role"), appBuilder =>
{
    appBuilder.Run(async context =>
    {
        await context.Response.WriteAsync($"<br> Role is {context.Request.Query["role"]}");
    });
});
```

<h3>app.Map()</h3>
Creates a branch in the request pipeline based on matches of the given request path
```
app.Map("/somePath", appBuilder => {
    appBuilder.Run(async ctx => {
        await ctx.Response.WriteAsync($"The path is {ctx.Request.Path}")
    })
});
```
It is possible to match sub-paths and therefore creating sub-branching

```
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
                ...
```
Here, when a patch is matched by `Map`, it becomes the `PathBase`.

<h5>app.MapWhen()</h5>
Conditionally creates a branch in the request pipeline that **does'nt return to the main pipeline.**
