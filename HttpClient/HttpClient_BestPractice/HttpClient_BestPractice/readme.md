
- HttpClient implements IDisposable
- unlike most disposable types in .NET the HttpClient should rarely be explicitly disposed
- When we dispose of the HttpClient the underlying HttpClientHandler is also disposed and the connection is closed (socket)


## HttpClientFactory

When the factory is called to create an instance of HttpClient the underlying HttpClientHandler is taken from a pool
Handlers exist in the pool for a default of 2 minutes before they are disposed

Using the factory, you can instantiate and configure as many light HttpClients as you need
since they use one of the pooled HttpMessageHandlers managed by the factory.

```
// Program.cs

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

// ItemService.cs

public class ItemService
{
    private readonly HttpClient _client;

    public ItemService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient();
        _client.BaseAddress = new Uri("http://localhost:5266/");
    }

    public async Task<Item?> GetItem(int id)
    {
        return await _client.GetFromJsonAsync<Item>($"items/{id}");
    }
}
```

## Use SendAsync
This method allows you to create a particular request object (HttpRequestMessage) that is then passed to the method.

## Always set the Accept request header
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


## Use Streams when reading responses

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


## Start reading response content ASAP

We can do this by passing a HttpCompletionOption argument through when calling SendAsync on the client:
```
var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
```

The response content should then be read as a stream

## Define custom Content types

```
public class JsonContent : StringContent
{
    public JsonContent(string content)
        : this(content, Encoding.UTF8)
    {
    }

    public JsonContent(string content, Encoding encoding)
        : base(content, encoding, "application/json")
    {
    }
}
```

These types can then be used on the HttpRequestMessage that we send from SendSync:

```
var request = new HttpRequestMessage(HttpMethod.Post, "api/orders");

request.Content = new JsonContent(json)
```

## Check the response status code

```
var response = await client.SendAsync(request);

if (response.StatusCode == HttpStatusCode.Unauthorized)
{
    // need to refresh the request security token!
}

if (response.IsSuccessStatusCode)
{
    // the status code was 2xx
}
```

## Check the response Content-Type

You may have set the Accept type on your client but that does not guarantee the API will “play ball” and return content in the data format you specified.
For this reason it is recommended that before examining/deserializing the response content you check the response’s content type is what you expect.

```
var response = await client.SendAsync(request);

if (response.Content.Headers.ContentType.MediaType == "application/json")
{
    var json = await response.Content.ReadAsStringAsync();
  
    var order = JsonConvert.DeserializeObject<Order>(json);
}
```

## Use Cancellation Tokens

```
public async Task<Order> GetOrderAsync(CancellationToken cancellationToken = default)
{
    // setup your client & request
  
    var response = await client.SendAsync(request, cancellationToken);
}
```
