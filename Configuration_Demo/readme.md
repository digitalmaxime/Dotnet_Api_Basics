Order of setting sources :

1 - CommandLine

2 - ENV Variables

3 - UserSecrets

4 - AppSettings.json (env specific)

5 - AppSettings.json (root level)


<h2>User Secrets</h2>
```
dotnet user-secrets init

dotnet user-secrets set mailpassword password1
```



## Getting configs through builder.Configuration

<h5>.GetValue<string></h5>
```
var validIssuerConfig = builder.Configuration.GetValue<string>("Security:ValidIssuer"); // Using nested Json 
```

<h5>["myConfig"]</h5>
```
var defaultLogLevel = builder.Configuration["Logging:LogLevel:Default"];
```

## Options pattern (preferred)

<h5>ConfigurationBinder.Bind</h5>

```
public class PositionOptions
{
    public const string Position = "Position";

    public string Title { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
}
```

```
var positionOptions = new PositionOptions();
        Configuration.GetSection(PositionOptions.Position).Bind(positionOptions);

        Console.WriteLine($"Title: {positionOptions.Title} \n" +
                       $"Name: {positionOptions.Name}");
```

---
ref : [microsoft doc](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0)