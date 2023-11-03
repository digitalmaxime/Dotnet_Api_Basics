using EfCore_MySql_CRUD.Domain;
using EfCore_MySql_CRUD.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCore_MySql_CRUD.Api;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonController : ControllerBase
{
    private readonly IPersonRepository _personRepository;

    public PersonController(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    [HttpGet]
    [Route("person/{id}")] // Path param style
    public async Task<Person> GetPersonById(int id)
    {
        return await Task.FromResult(new Person() { Id = id });
    }

    [HttpGet]
    [Route("person/name")] // Query param style
    public async Task<Person> GetPersonByName(string name)
    {
        return await Task.FromResult(new Person() { Id = -1, Name = name });
    }

    [HttpPost]
    [ActionName("Index")]
    [Route("person")]
    public async Task<IActionResult> CreatePerson(string name)
    {
        var res = await _personRepository.CreatePerson(new Person() { Name = name });
        if (res == null)
        {
            return NotFound($"Error creating person with name {name}");
        }

        return Ok(res);
    }
}