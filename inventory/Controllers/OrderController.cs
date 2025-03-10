namespace inventory.Controllers;
using System;
using inventory.Models.Orders;
using inventory.Services.OrderRepo;
using Microsoft.AspNetCore.Mvc;

public class OrderController : Controller
{
    public readonly IOrderService _orderService;   
    public readonly ILogger<OrderController> _logger;

    // Constructor to initialize the order service and logger
    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }   

    // Action method to display the list of orders with optional search and date filters
    public async Task<IActionResult> Index(string? query, DateTime? startDate, DateTime? endDate)
    {
        IEnumerable<Order> orders = null!;
        try
        {
            // Retrieve all orders from the service
            var allOrders = await _orderService.GetOrders();

            // Filter orders based on the search query
            if (!string.IsNullOrEmpty(query))
            {
                orders = allOrders?.Where(o => o.Id.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ?? Enumerable.Empty<Order>();
            }
            else
            {
                orders = allOrders;
            }

            // Filter orders based on order date range
            if (startDate.HasValue && endDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate.Date >= startDate.Value.Date && o.OrderDate.Date <= endDate.Value.Date);
            }
            else if (startDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate.Date >= startDate.Value.Date);
            }
            else if (endDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate.Date <= endDate.Value.Date);
            }

            // Store search parameters in ViewData for use in the view
            ViewData["SearchQuery"] = query;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur during the process
            _logger.LogWarning($"Failure to access order service: {ex.Message}");
            orders = Array.Empty<Order>();
        }

        // Return the view with the filtered orders
        return View(orders);
    }

    // Action method to display the details of a specific order
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            // Retrieve the order by its ID
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Return the view with the order details
            return View(order);
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur during the process
            _logger.LogWarning($"Failure to access order details: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}