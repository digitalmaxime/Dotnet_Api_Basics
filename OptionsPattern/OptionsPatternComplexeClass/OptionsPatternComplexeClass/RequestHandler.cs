using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using OptionsPatternComplexeClass.Options;

namespace OptionsPatternComplexeClass;

public interface IRequestHandler
{
    Results<NotFound, Ok<HelloWorldDto>> ListAllHttpClientOptions();
    Results<NotFound, Ok<List<string>>> ListAllSystemOptions();
}

public class RequestHandler : IRequestHandler
{
    private readonly SystemsOption _systemOption;
    private readonly HttpClientSecurityOption _httpClientOption;

    public RequestHandler(IOptions<HttpClientSecurityOption> httpClientOption, IOptions<SystemsOption> systemOption)
    {
        _systemOption = systemOption.Value;
        _httpClientOption = httpClientOption.Value;
    }

    public Results<NotFound, Ok<HelloWorldDto>> ListAllHttpClientOptions()
    {
        var clientUserNames = new [] { _httpClientOption.Client1?.UserName, _httpClientOption.Client2?.UserName, _httpClientOption.Client3?.UserName };

        var responseDto = new HelloWorldDto()
        {
            Message = "Hello World from minimal APi!",
            ClientId = _httpClientOption.ClientId,
            ClientUsername = clientUserNames
        };

        return TypedResults.Ok(responseDto);
    }
    
    public Results<NotFound, Ok<List<string>>> ListAllSystemOptions()
    {
        var allSystems = new List<string>();
        foreach (var system in _systemOption.Dict)
        {
            var uri = system.Value;
            allSystems.Add(uri.ToString());
        }
        
        return TypedResults.Ok(allSystems);
    }
}

public class HelloWorldDto
{
    public string Message { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string?[]? ClientUsername { get; set; }
}