using BethanysPieShopAdmin.Models;
using BethanysPieShopAdmin.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Repositories;

public class CategoryRepository: ICategoryRepository
{
    private readonly BethanysPieShopDbContext _bethanysPieShopDbContext;
    
    public CategoryRepository(BethanysPieShopDbContext bethanysPieShopDbContext)
    {
        _bethanysPieShopDbContext = bethanysPieShopDbContext;
    }
    
    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        return await _bethanysPieShopDbContext.Categories.OrderBy(c => c.CategoryId).ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _bethanysPieShopDbContext.Categories
            .Include(c => c.Pies)
            .FirstOrDefaultAsync(c => c.CategoryId == id);
    }
}