namespace EFCore_JulieLerman.Dtos;

public record AuthorDto()
{
    public required string  Name { get; init; }
    public ICollection<BookDto> Books { get; init; } = new List<BookDto>();
}