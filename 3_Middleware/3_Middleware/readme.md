# Middleware and Filters

This demo shows how to use middleware and filters in ASP.NET Core using minimal APIs.

It features: Response caching, Logging, Error handling

## Application request pipeline

Three main methods to configure the application's request processing pipeline

### `app.Use()`
adds a middleware delegate to the pipeline that can perform processing both before and after the subsequent middleware in the chain. 
It explicitly receives a next delegate, which it must call (e.g., `await next()`) to pass the request to the next component in the pipeline. 
This makes it suitable for "wrapping" logic like logging, authentication, or error handling that needs to interact with the entire request lifecycle.

### `app.Run()`
adds a terminal middleware delegate that always short-circuits the pipeline. 
It does not have a next delegate parameter and, once executed, 
it writes directly to the response and prevents any later middleware components

### `app.Map()`
is used to branch the middleware pipeline based on a matching request path. 
If an incoming request's URL path starts with the specified pattern (e.g., /api), the request is routed into a separate, modular pipeline defined within the Map delegate

### Middelwares

Middleware is software that's assembled into an app pipeline to handle requests and responses.
Commonly used predefined middleware components include:
- Authentication and Authorization middleware
- Error handling middleware
- CORS middleware
- Rate limiting
- Response caching 
  - ** UseCors must be called before UseResponseCaching when using CORS middleware.

### Filters

Minimal API filters allow developers to implement business logic that supports:

- Running code before and after the endpoint handler.
- Inspecting and modifying parameters provided during an endpoint handler invocation.
- Intercepting the response behavior of an endpoint handler. 
 
Filters can be helpful in the following scenarios:

- Validating the request parameters and body that are sent to an endpoint.
- Logging information about the request and response.
- Validating that a request is targeting a supported API version.

--- 

references:
- [ASP.NET Core Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0)
- [ASP.NET Core Filters](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/min-api-filters?view=aspnetcore-10.0)
