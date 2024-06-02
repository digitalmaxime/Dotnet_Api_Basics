using BethanysPieShopAdmin.Models;
using BethanysPieShopAdmin.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly BethanysPieShopDbContext _bethanysPieShopDbContext;

    public OrderRepository(BethanysPieShopDbContext bethanysPieShopDbContext)
    {
        _bethanysPieShopDbContext = bethanysPieShopDbContext;
    }

    public async Task<Order?> GetOrderDetailsAsync(int orderId)
    {
        return await _bethanysPieShopDbContext.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Pie)
            .OrderBy(o => o.OrderId)
            // .Where(o => o.OrderId == orderId)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsASync()
    {
        return await _bethanysPieShopDbContext.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Pie)
            .ToListAsync();

    }
}