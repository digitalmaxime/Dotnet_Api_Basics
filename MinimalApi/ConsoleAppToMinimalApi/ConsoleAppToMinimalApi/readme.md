Starting from ConsoleApp


The WebApi has 2 fondamentals components: 
- The WebApplicationBuilder
  - Where all Api configs live
- The Web Application itself
  - Where all middlewares are configured
  - 
### Change csproj 

`<Project Sdk="Micrososft.NET.Sdk.Web">`

### Scafold the builder + app 

`var builder = WebApplication.CreateBuilder(args);`

`// treat this like the ConfigureServices <--`

`var app = builder.Build();`

`// endpoints goes here <--`

`// treat this like the Configure in startup.cs <--`

`app.Run();`

### Adding endpoints

After `var app = builder.Build();` and before `app.Run();`

`app.MapGet("hello", () => "Hello World!");`

### Running the app

`dotnet watch run --no-hotreload`

"Everytime the file is updated, the app will restart"