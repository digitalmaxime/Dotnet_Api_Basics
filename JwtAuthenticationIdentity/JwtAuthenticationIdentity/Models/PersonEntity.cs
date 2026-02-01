namespace JwtAuthenticationIdentity.Models;

public class Person
{
    public int Id { get; init; }
    
    public required string Name { get; set; }
    
    public string? Email { get; set; }
}