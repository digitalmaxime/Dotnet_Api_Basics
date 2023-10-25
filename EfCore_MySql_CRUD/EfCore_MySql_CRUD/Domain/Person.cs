namespace EfCore_MySql_CRUD.Domain;

public record Person
{
    public int Id { get; init; }
    public string? Name { get; init; }
}