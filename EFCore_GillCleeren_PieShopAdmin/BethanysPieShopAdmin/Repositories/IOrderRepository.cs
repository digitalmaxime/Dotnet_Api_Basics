using BethanysPieShopAdmin.Models;

namespace BethanysPieShopAdmin.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetOrderDetailsAsync(int orderId);
    Task<IEnumerable<Order>> GetAllOrdersWithDetailsASync();
}