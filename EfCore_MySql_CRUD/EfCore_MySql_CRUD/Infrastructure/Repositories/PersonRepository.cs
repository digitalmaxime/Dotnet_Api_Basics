using EfCore_MySql_CRUD.Domain;
using Microsoft.EntityFrameworkCore;

namespace EfCore_MySql_CRUD.Infrastructure.Repositories;

public interface IPersonRepository
{
    Task<Person?> CreatePerson(Person person);
    Task<Person?> GetPersonById(int personId);
    Task<Person?> DeletePersonById(int personId);
    Task<Person?> UpdatePerson(int personId, string name);
    Task<Person?> GetPersonByName(string name);
}

public class PersonRepository : IPersonRepository
{
    private readonly PersonContext _context;
    private readonly ILogger<PersonContext> _logger;

    public PersonRepository(PersonContext context, ILogger<PersonContext> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Person?> CreatePerson(Person person)
    {
        _logger.LogInformation("Creating a person with name {name}", person.Name);
        _context.Add(person);
        
        var nbOfUpdates = await _context.SaveChangesAsync();

        return nbOfUpdates > 0 ? person : null;
    }

    public async Task<Person?> GetPersonById(int personId)
    {
        return await _context.FindAsync<Person>(personId);
    }

    public async Task<Person?> GetPersonByName(string name)
    {
        return await _context.Person.Where(x => x.Name == name).FirstOrDefaultAsync();
    }

    public async Task<Person?> DeletePersonById(int personId)
    {
        var person = _context.Find<Person>(personId);
        if (person != null)
        {
            _context.Remove(person);
            await _context.SaveChangesAsync();
            return person;
        }

        return null;
    }
    
    public async Task<Person?> UpdatePerson(int personId, string name)
    {
        var person = _context.Find<Person>(personId);
        if (person != null)
        {
            var nbUpdates = await _context.Set<Person>()
                .Where(p => p.Id == personId)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.Name, name));

            if (nbUpdates > 0)
            {
                await _context.Entry(person).ReloadAsync();
                var updatedPerson = _context.Find<Person>(personId);

                return updatedPerson;
            }
        }

        return null;
    }
}