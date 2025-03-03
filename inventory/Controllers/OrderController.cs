namespace inventory.Controllers;
using System;
using inventory.Models.Orders;
using inventory.Services.OrderRepo;
using Microsoft.AspNetCore.Mvc;

public class OrderController:Controller{

    public readonly IOrderService _orderService;   
    public readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger){
        _orderService = orderService;
        _logger = logger;
    }   

    public async Task<IActionResult> Index(string? query, DateTime? startDate, DateTime? endDate){
        
        IEnumerable<Order> orders = null!;
        try{
            var allOrders = await _orderService.GetOrders();
            if (!string.IsNullOrEmpty(query)){
                orders = allOrders?.Where(o => o.Id.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ?? Enumerable.Empty<Order>();
            }
            else{
                orders = allOrders;
            }

            // Filter orders based on order date range
            if (startDate.HasValue && endDate.HasValue){
                orders = orders.Where(o => o.OrderDate.Date >= startDate.Value.Date && o.OrderDate.Date <= endDate.Value.Date);
            }
            else if (startDate.HasValue){
                orders = orders.Where(o => o.OrderDate.Date >= startDate.Value.Date);
            }
            else if (endDate.HasValue){
                orders = orders.Where(o => o.OrderDate.Date <= endDate.Value.Date);
            }

            ViewData["SearchQuery"] = query;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
        }
        catch (Exception ex){
            _logger.LogWarning($"failure to access order service : {ex.Message}");
            orders = Array.Empty<Order>();
        }
        Console.WriteLine(orders);
        return View(orders);
    }

    public async Task<IActionResult> Details(int? id){
        if (id == null){
            return NotFound();
        }
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null){
            return NotFound();
        }
        return View(order);
    }
    
}