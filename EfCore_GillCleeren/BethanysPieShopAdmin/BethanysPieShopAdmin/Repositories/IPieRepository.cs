using BethanysPieShopAdmin.Models;

namespace BethanysPieShopAdmin.Repositories;

public interface IPieRepository
{
    Task<IEnumerable<Pie>> GetAllPiesAsync();
    Task<Pie?> GetPieByIdAsync(int pieId);

    Task<int> AddPieAsync(Pie pie);
}