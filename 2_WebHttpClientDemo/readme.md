# Http Client Demo

The http client is relying on the preceding demo ***1_WebConfigurationDemo***

### The IHttpClientFactory type

When you call any of the AddHttpClient extension methods, you're adding the IHttpClientFactory and related services to the IServiceCollection. 
The IHttpClientFactory type offers the following benefits:

- Exposes the HttpClient class as a DI-ready type.
- Provides a central location for naming and configuring logical HttpClient instances.
- Codifies the concept of outgoing middleware via delegating handlers in HttpClient.
- Provides extension methods for Polly-based middleware to take advantage of delegating handlers in HttpClient.
- Manages the caching and lifetime of underlying HttpClientHandler instances. Automatic management avoids common Domain Name System (DNS) problems that occur when manually managing HttpClient lifetimes.
- Adds a configurable logging experience (via ILogger) for all requests sent through clients created by the factory.

### Consumption patterns

There are several ways IHttpClientFactory can be used in an app:

- Basic usage
- Named clients
  - The app requires many distinct uses of HttpClient.
  - Many HttpClient instances have different configurations.
- Typed clients
  - The HttpClient is assigned as a class-scoped variable (field) 
  - For a single backend endpoint.
  - To encapsulate all logic dealing with the endpoint.
  - Using typed clients in singleton services can be dangerous. For more information, see the [Avoid Typed clients in singleton services section](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#avoid-typed-clients-in-singleton-services).
- Generated clients

### Packages

- `Microsoft.Extensions.Http`
- `Refit.HttpClientFactory` (optional)

### Serialization

use `new JsonSerializerOptions(JsonSerializerDefaults.Web)`

---

reference: [learn microsoft .NET httpclient](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory)