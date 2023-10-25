using EfCore_MySql_CRUD.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EfCore_MySql_CRUD.Api;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonController: ControllerBase
{
    [HttpGet]
    [Route("person/{id}")] // Path param style
    public async Task<Person> Person(int id)
    {
        return await Task.FromResult(new Person() { Id = id });
    }
    
    [HttpGet]
    [Route("person/name")] // Query param style
    public async Task<Person> Person(string name)
    {
        return await Task.FromResult(new Person() { Id = -1, Name = name });
    }
}