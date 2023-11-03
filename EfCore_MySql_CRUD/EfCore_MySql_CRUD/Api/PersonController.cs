using System.Net;
using EfCore_MySql_CRUD.Domain;
using EfCore_MySql_CRUD.Infrastructure;
using EfCore_MySql_CRUD.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<Person>> GetPersonById(int id)
    {
        var res = await _personRepository.GetPersonById(id);
        if (res == null)
        {
            return NotFound($"Did not find any person with id : {id}");
        }

        return Ok(res);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Person), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("person/name")] // Query param style
    public async Task<ActionResult<Person>> GetPersonByName(string name)
    {
        var res = await _personRepository.GetPersonByName(name);
        if (res == null)
        {
            return NotFound($"Did not find any person with name : {name}");
        }

        return Ok(res);
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

    [HttpDelete]
    [Route("person")]
    public async Task<IActionResult> DeletePerson(int id)
    {
        var res = await _personRepository.DeletePersonById(id);
        if (res == null)
        {
            return NotFound($"Error creating person with id {id}");
        }

        return Ok(new { Message = $"Successfully delete person with id {id}", Entity = res });
    }
    
    [HttpPatch]
    [Route("person")]
    public async Task<IActionResult> ModifyPerson(int id, string name)
    {
        var res = await _personRepository.UpdatePerson(id, name);
        if (res == null)
        {
            return NotFound($"Error modifying person with id {id}");
        }

        return Ok(new { Message = $"Successfully modified person with id {id}", ModifiedPerson = res });
    }
}