# Http Client Demo

The http client is relying on the preceding demo ***1_WebConfigurationDemo***

### The IHttpClientFactory type

Using the factory, you can instantiate and configure as many light HttpClients as you need since they use one of the
pooled HttpMessageHandlers managed by the factory.

```csharp
public Ctor(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient();
        _client.BaseAddress = new Uri("http://localhost:8080/");
    }
```

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
  - doesn't require an http factory 
  - The HttpClient is assigned as a class-scoped variable (field) 
  - For a single backend endpoint.
  - To encapsulate all logic dealing with the endpoint.
  - Using typed clients in singleton services can be dangerous. For more information, see the [Avoid Typed clients in singleton services section](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#avoid-typed-clients-in-singleton-services).
- Generated clients

### Packages

- `Microsoft.Extensions.Http`
- `Refit.HttpClientFactory` (optional)
- `Microsoft.Extensions.Http.Resilience` for retry policies

### Best Practices

#### Serialization

use `new JsonSerializerOptions(JsonSerializerDefaults.Web)`

#### Use retry policies 



#### Always set the Accept request header

Setting the Accept header on the request with a content MIME type (such as application/json ) allows the API to know what format the client expects the response content to be in

The Accept request header can either be set on the HttpClient:
```
var mt = new MediaTypeWithQualityHeaderValue("application/json");

client.DefaultRequetHeaders.Accept.Add(mt);
```

Or can be set on the request object itself:

```
var request = new HttpRequestMessage(HttpMethod.Get, "api/orders");

var mt = new MediaTypeWithQualityHeaderValue("application/json");

request.Headers.Accept.Add(mt);
```


#### Use Streams when reading responses if possible

```
var response = await client.SendAsync(request);

using(var stream = await response.Content.ReadAsStreamAsync())
{
    using (var streamReader = new StreamReader(stream))
    {
      using (var jsonTextReader = new JsonTextReader(streamReader))
      {
        var customer = new JsonSerializer().Deserialize<Customer>(jsonTextReader);      
	  
        // do something with the customer
      }
    }
}
```

#### Start reading response content ASAP

We can do this by passing a HttpCompletionOption argument through when calling SendAsync on the client:
```
var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
```

The response content should then be read as a stream

#### Use Cancellation Tokens

```
public async Task<Order> GetOrderAsync(CancellationToken cancellationToken = default)
{
    // setup your client & request
  
    var response = await client.SendAsync(request, cancellationToken);
}
```


---

reference: [learn microsoft .NET httpclient](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory)