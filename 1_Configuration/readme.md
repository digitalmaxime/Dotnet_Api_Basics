## Configuration hierarchy

1 - CommandLine 
- ` dotnet run --environment MyCustomSetting="MyCustomSetting from command line"   `

2 - ENV Variables
- can be set in /Properties/launchSettings.json `environmentVariables`

3 - UserSecrets
- `dotnet user-secrets init`
- `dotnet user-secrets set MyCustomSetting "MyCustomSetting from secrets"`

4 - appsettings.Development.json (env specific)

5 - appsettings.json (generic)

Code snipet to retrieve configuration object: `configuration.GetSection("sectionname").Get<MyClass>();`

## Options pattern

Options pattern allows for dependency injection with configuration typed objects.

It works like AddSingleton but fills in the properties {get; set;} with elements found in `Configuration.GetSection(...)`

`builder.Services.AddOptions<MySettings>().Bind(configuration.GetSection("MySettings1"));`

### 

references : 
- configuration: [.Net configurations](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration)
- options pattern: [.Net options pattern](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-10.0)