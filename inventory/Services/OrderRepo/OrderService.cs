using inventory.Data;
using inventory.Models;
using inventory.Models.Orders;
using inventory.Services.AnalyticsRepo;
using inventory.Services.AnalyticsResp;
using Microsoft.EntityFrameworkCore;

namespace inventory.Services.OrderRepo
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly ProductContext _context;

        public OrderService(ILogger<OrderService> logger, ProductContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Save changes to the database
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save changes");
                return false;
            }
        }

        // Update an existing order
        public async Task<bool> UpdateOrder(Order order)
        {
            try
            {
                _context.Entry(order).State = EntityState.Modified;
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update order");
                return false;
            }
        }

        // Remove an order
        public async Task<bool> RemoveOrder(Order order)
        {
            try
            {
                _context.Orders.Remove(order);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove order");
                return false;
            }
        }

        // Check if an order exists by ID
        public async Task<bool> OrderExists(int orderId)
        {
            try
            {
                return await _context.Orders.AnyAsync(e => e.Id == orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check if order exists");
                return false;
            }
        }

        // Get an order by ID
        public async Task<OrderViewModel?> GetOrderByIdAsync(int? orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Product!)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogError($"Order with id {orderId} not found");
                    return null;
                }

                return new OrderViewModel
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    Items = order.OrderItems?.Select(oi => new OrderItemViewModel
                    {
                        OrderItemId = oi.Id,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product?.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Product?.Price ?? 0
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order by ID");
                return null;
            }
        }

        // Get all orders
        public async Task<IEnumerable<Order>> GetOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Product)
                    .ToListAsync();

                return orders.Select(o => new Order
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    OrderItems = o.OrderItems?.Select(oi => new OrderItem
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        OrderId = oi.OrderId,
                        Product = oi.Product,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get orders");
                return new List<Order>();
            }
        }

        // Add a new order
        public async Task<bool> AddOrder(OrderViewModel orderViewModel)
        {
            try
            {
                var order = new Order
                {
                    OrderDate = orderViewModel.OrderDate,
                    OrderItems = orderViewModel.Items?.Select(oi => new OrderItem
                    {
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add order");
                return false;
            }
        }

        // Update an existing order using OrderViewModel
        public async Task<bool> UpdateOrder(OrderViewModel orderViewModel)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderViewModel.OrderId);

                if (order == null)
                {
                    _logger.LogError($"Order with id {orderViewModel.OrderId} not found");
                    return false;
                }

                order.OrderDate = orderViewModel.OrderDate;

                if (order.OrderItems == null)
                {
                    _logger.LogError($"Order with id {orderViewModel.OrderId} has no order items");
                    return false;
                }

                foreach (var orderItem in order.OrderItems)
                {
                    var orderItemViewModel = orderViewModel.Items?.FirstOrDefault(oi => oi.OrderItemId == orderItem.Id);

                    if (orderItemViewModel == null)
                    {
                        _logger.LogError($"OrderItem with id {orderItem.Id} not found");
                        return false;
                    }

                    orderItem.Quantity = orderItemViewModel.Quantity;
                    orderItem.Price = orderItemViewModel.Price;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update order");
                return false;
            }
        }

        // Get the most recent 5 orders
        public async Task<IEnumerable<Order>> GetRecentOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync();

                return orders.Select(o => new Order
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    OrderItems = o.OrderItems?.Select(oi => new OrderItem
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        OrderId = oi.OrderId,
                        Product = oi.Product,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recent orders");
                return new List<Order>();
            }
        }

        // Get the total number of orders
        public Task<int> GetOrderCount()
        {
            try
            {
                return _context.Orders.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order count");
                return Task.FromResult(0);
            }
        }

        // Get the number of orders placed today
        public Task<int> GetTodaySalesCount()
        {
            try
            {
                return _context.Orders
                    .Where(o => o.OrderDate.Date == DateTime.Today)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get today's sales count");
                return Task.FromResult(0);
            }
        }

        // Get the total sales value for today
        public Task<decimal> GetTodaySalesValue()
        {
            try
            {
                return _context.Orders
                    .Where(o => o.OrderDate.Date == DateTime.Today)
                    .SumAsync(o => o.TotalPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get today's sales value");
                return Task.FromResult(0m);
            }
        }

        // Get the top 5 selling products
        public async Task<IEnumerable<Product>> GetTopSellingProducts()
        {
            try
            {
                var products = await _context.OrderItems
                    .GroupBy(oi => oi.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Quantity = g.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(g => g.Quantity)
                    .Take(5)
                    .ToListAsync();

                return products.Select(p => new Product
                {
                    ProductId = p.ProductId,
                    Name = _context.Product.Find(p.ProductId)?.Name,
                    Quantity = p.Quantity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get top selling products");
                return new List<Product>();
            }
        }

        // Get the top 5 selling products with total quantity
        public async Task<IEnumerable<Product>> GetTopSellingProductsWithTotalQuantity()
        {
            try
            {
                var products = await _context.OrderItems
                    .GroupBy(oi => oi.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Quantity = g.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(g => g.Quantity)
                    .Take(5)
                    .ToListAsync();

                return products.Select(p => new Product
                {
                    ProductId = p.ProductId,
                    Name = _context.Product.Find(p.ProductId)?.Name,
                    Quantity = p.Quantity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get top selling products with total quantity");
                return new List<Product>();
            }
        }

        // Get the bottom 5 selling products
        public async Task<IEnumerable<Product>> GetLowSellingProducts()
        {
            try
            {
                var products = await _context.OrderItems
                    .GroupBy(oi => oi.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Quantity = g.Sum(oi => oi.Quantity)
                    })
                    .OrderBy(g => g.Quantity)
                    .Take(5)
                    .ToListAsync();

                return products.Select(p => new Product
                {
                    ProductId = p.ProductId,
                    Name = _context.Product.Find(p.ProductId)?.Name,
                    Quantity = p.Quantity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get low selling products");
                return new List<Product>();
            }
        }

        // Get the total sales value for the past week
        public async Task<decimal> GetWeekSalesValue()
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.OrderDate.Date >= DateTime.Today.AddDays(-7))
                    .SumAsync(o => o.TotalPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get week sales value");
                return 0m;
            }
        }

        // Get the total sales value for the past month
        public async Task<decimal> GetMonthSalesValue()
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.OrderDate.Date >= DateTime.Today.AddMonths(-1))
                    .SumAsync(o => o.TotalPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get month sales value");
                return 0m;
            }
        }

        // Get the sales trend data
        public async Task<List<SalesTrendDto>> GetSalesTrend()
        {
            try
            {
                var trends = await _context.Orders
                    .GroupBy(o => o.OrderDate.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new SalesTrendDto
                    {
                        Date = g.Key.ToString("yyyy-MM-dd"),
                        TotalSales = g.Sum(o => o.TotalPrice)
                    })
                    .ToListAsync();

                if (!trends.Any())
                {
                    _logger.LogInformation("No sales trend data found in Orders table");
                }
                return trends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve sales trend data");
                return new List<SalesTrendDto>();
            }
        }

        // Get the monthly sales list
        public async Task<List<decimal>> GetMonthlySalesList()
        {
            try
            {
                return await _context.Orders
                    .GroupBy(o => o.OrderDate.Month)
                    .Select(g => g.Sum(o => o.TotalPrice))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve monthly sales data");
                return new List<decimal>();
            }
        }

        // Get the weekly sales list
        public async Task<List<decimal>> GetWeeklySalesList()
        {
            try
            {
                var orders = await _context.Orders
                    .Select(o => new { o.OrderDate, o.TotalPrice })
                    .ToListAsync(); // Fetch data into memory

                return orders
                    .GroupBy(o => o.OrderDate.DayOfWeek)
                    .OrderBy(g => g.Key) // Optional: Sort by DayOfWeek enum (Sunday to Saturday)
                    .Select(g => g.Sum(o => o.TotalPrice))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve weekly sales data");
                return new List<decimal>();
            }
        }

        // Get the daily sales list
        public async Task<List<decimal>> GetDailySalesList()
        {
            try
            {
                return await _context.Orders
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => g.Sum(o => o.TotalPrice))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve daily sales data");
                return new List<decimal>();
            }
        }

        // Get the sales data by category
        public async Task<List<CategorySalesDto>> GetCategorySales()
        {
            try
            {
                var categorySales = await _context.Orders
                    .Join(_context.OrderItems, o => o.Id, oi => oi.OrderId, (o, oi) => new { o, oi })
                    .Join(_context.Product, oi => oi.oi.ProductId, p => p.ProductId, (oi, p) => new { oi.o.TotalPrice, p.CategoryId })
                    .GroupBy(x => (int?)(x.CategoryId) ?? 0) // 0 for null categories
                    .Select(g => new
                    {
                        CategoryId = g.Key,
                        Sales = g.Sum(x => x.TotalPrice)
                    })
                    .Join(_context.Categories,
                          cs => cs.CategoryId,
                          c => c.CategoryId,
                          (cs, c) => new CategorySalesDto
                          {
                              Category = c != null ? c.Name : "Unknown",
                              Sales = cs.Sales
                          })
                    .ToListAsync();

                if (!categorySales.Any())
                {
                    _logger.LogInformation("No category sales data found");
                }
                return categorySales;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve category sales data");
                return new List<CategorySalesDto>();
            }
        }
    }
}