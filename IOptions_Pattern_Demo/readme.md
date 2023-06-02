<h2>IOptions </h2>
Magic code in startup (program.cs)
```
builder.Services
    .Configure<WeatherApiOptions>(builder.Configuration.GetSection(WeatherApiOptions.WeatherApi));
```

Above code works like `AddSingleton` but fills in the properties `{get; set;}`
with elements found in `Configuration.GetSection(..)`.

_(If you want more of a scoped like service, use IOptionsSnapshot. 
This will have options recomputed on every request.
Therefore, it will pickup changes in the appSettings.json without having to rebuild the app.
It cannot be injected into a Singleton service)_

Nice so it allows for DI the an instance with properties populated from appsettings.json config file!

Next up, inject IOptions through regular DI
```
class MyClass {
    private readonly WeatherApiOptions _weatherOptions;

    public WeatherForecastController(
        IOptions<WeatherApiOptions> weatherOptions
        )
    {
        _weatherOptions = weatherOptions.Value;
    }
    
    public async Task<string> Get()
    {
        var baseUrl = _weatherOptions.Url;
        ...
    }
}
```

---
<h2>Validation</h2>
With `using System.ComponentModel.DataAnnotations;`
we can annotate the Option class public properties.
E.g 
```
[Required]
public string Url { get; set; } 
```

In startup (program.cs) we can ensure that the magically filled in properties are validated.
```
builder.Services.AddOptions<WeatherApiOptions>()
    .Bind(builder.Configuration.GetSection(WeatherApiOptions.WeatherApi))
    .ValidateDataAnnotations();
```

---
<h2>Rules</h2>
- An Option class should be non-abstract
- An Option class should have a public default constructor
- All public read/write `{ get; set; }` Properties are bound to the configuration value
- The _field values do not get bound
  (e.g `    public const string WeatherApi = "WeatherApi";`)

---
ref : https://www.youtube.com/watch?v=SizJCLcjbOA&ab_channel=RahulNath