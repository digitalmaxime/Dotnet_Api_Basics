using BethanysPieShopAdmin.Models;
using BethanysPieShopAdmin.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Repositories;

public class PieRepository : IPieRepository
{
    private readonly BethanysPieShopDbContext _context;

    public PieRepository(BethanysPieShopDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pie>> GetAllPiesAsync()
    {
        return await _context.Pies.OrderBy(x => x.PieId).AsNoTracking().ToListAsync();
    }

    public async Task<Pie?> GetPieByIdAsync(int pieId)
    {
        return await _context.Pies
            .Include(p => p.Category)
            .Include(p => p.Ingredients)
            .AsNoTracking() 
            .FirstOrDefaultAsync(p => p.PieId == pieId);
    }

    public async Task<int> AddPieAsync(Pie pie)
    {
        _context.Pies.Add(pie);
        return await _context.SaveChangesAsync();
    }
}