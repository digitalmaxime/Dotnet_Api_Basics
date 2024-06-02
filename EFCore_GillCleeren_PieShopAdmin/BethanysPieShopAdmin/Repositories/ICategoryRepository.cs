using BethanysPieShopAdmin.Models;

namespace BethanysPieShopAdmin.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllCategories();
    Task<Category?> GetCategoryByIdAsync(int id);
}