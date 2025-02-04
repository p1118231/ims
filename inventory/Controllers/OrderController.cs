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

    public async Task<IActionResult> Index(string? query){
        
        IEnumerable<Order> orders = null!;
        try{
            if (!string.IsNullOrEmpty(query)){
                var allOrders = await _orderService.GetOrders();
                orders = allOrders?.Where(o => o.Id.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ?? Enumerable.Empty<Order>();
            }
            else{
                orders = await _orderService.GetOrders();
            }
            ViewData["SearchQuery"] = query;
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