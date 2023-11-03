using Microsoft.AspNetCore.Mvc;

namespace MinimalApiStructured.Endpoints;

[HttpPost("customers", AllowAnonymous)]
public class CreateCustomerEndpoint : Endpoint
{
    public CreateCustomerEndpoint(RequestDelegate? requestDelegate, EndpointMetadataCollection? metadata, string? displayName) : base(requestDelegate, metadata, displayName)
    {
    }
    
    public override async Task HandleAsync(CreateCustomerRequest)
}