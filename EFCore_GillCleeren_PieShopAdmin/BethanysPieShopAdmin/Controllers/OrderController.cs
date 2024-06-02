using BethanysPieShopAdmin.Models;
using BethanysPieShopAdmin.Repositories;
using BethanysPieShopAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BethanysPieShopAdmin.Controllers;

public class OrderController : Controller
{
    private readonly IOrderRepository _orderRepository;

    public OrderController(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    // GET
    public async Task<IActionResult> Index(int? orderId, int? orderDetailId)
    {
        OrderIndexViewModel orderIndexViewModel = new()
        {
            Orders = await _orderRepository.GetAllOrdersWithDetailsASync()
        };

        if (orderId != null)
        {
            Order selectedOrder = orderIndexViewModel.Orders.Single(o => o.OrderId == orderId);
            orderIndexViewModel.OrderDetails = selectedOrder.OrderDetails;
            orderIndexViewModel.SelectedOrderId = orderId;
        }

        if (orderDetailId != null)
        {
            var selectedOrderDEtail = orderIndexViewModel.OrderDetails?.Single(od => od.OrderDetailId == orderDetailId);
            orderIndexViewModel.Pies = new List<Pie?>() { selectedOrderDEtail?.Pie };
            orderIndexViewModel.SelectedOrderDetailId = orderDetailId;

        }


        return View(orderIndexViewModel);
    }
    
    public async Task<IActionResult> Details(int? orderId)
    {
        var result = await _orderRepository.GetOrderDetailsAsync(orderId ?? 0);

        return View(result);
    }
}