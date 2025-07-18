using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book?>> GetBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book?> GetBookByTitleAsync(string title);
}

public class BookRepository : IBookRepository
{
    private readonly LibraryContext _context;

    public BookRepository(LibraryContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Book?>> GetBooksAsync()
    {
        return await _context.Books.ToListAsync();
    }
    
    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _context.Books.FindAsync(id);
    }
    
    public async Task<Book?> GetBookByTitleAsync(string title)
    {
        return await _context.Books
            .Include(x => x.Cover)
            .FirstOrDefaultAsync(b => b.Title == title);
    }
}