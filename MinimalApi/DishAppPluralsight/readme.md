<h1>Dish minimal Api</h1>

<h2>Routing</h2>
<h2>Endpoint Builder</h2>
<h2>CRUD</h2>
<h2>Handling Exception and Logging</h2>
<h2>Fluent Validation</h2>
include ```FluentValidation```

<h2>Token-based Security</h2>
<h3>IAuthenticationService</h3>

`builder.Services.AddAuthentication().AddJwtBearer();`

nuget --> Microsoft.AspNetCore.Authentication.JwtBearer

N.B. Options can be set with .AddJwtBearer(opt => ...) but they can also be set through _appSettings.json_ "Authentication": {...}

<h3>IAuthorizationService</h3>
`builder.Services.AddAuthorization();`

```
var ingredientsEndpoints = endpointRouterBuilder
    .MapGroup("/dishes/{dishId:guid}/ingredients")
    .RequireAuthorization();
```

Generate test token with dotnet cli

`dotnet user-jwts create --help`
`dotnet user-jwts create --audience menu-api --claim country=Belgium --role admin`
`dotnet user-jwts print`

---

<h2>Documenting Api</h2>

Swashbuckle.AspNetCore is a library that inspects the api code and generates an openApi formatted specification.
Swashbuckle UI wraps swagger-ui, a documentation interface.

Support for improving the OpenAPI specification is provided via `Microsoft.AspNetCore.OpenApi`

**nuget** : `Swashbuckle.AspNetCore` `Microsoft.AspnetCore.OpenApi`

`builder.Services.AddEndpointsApiExplorer();`
its an abstraction that exposes metadata information about the api

AddEndpointsApiExplorer is for Minimal APIs whereas AddApiExplorer is for MVC Core.

N.B. _For API projects, AddControllers calls AddApiExplorer on your behalf._

`builder.Services.AddSwaggerGen();`
Swashbuckle uses the metadata exposed by AddEndpointsApiExplorer to discover and describe the api with default annotation

_Microsoft.AspnetCore.OpenApi_ allows to do this :
```
dishWithGuidIdEndpoints.MapGet("", DishesHandlers.GetDishByIdAsync)
            .WithName("GetDish")
            .WithOpenApi()
            .WithSummary("Get a dish by providing an id.")
            .WithDescription("Blablablablablabla");
```

--- 

ref : Pluralsight Course _Build ASP.NET Core 7 Minimal APIs_ by Kevin Dockx