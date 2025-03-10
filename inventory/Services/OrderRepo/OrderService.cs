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

        public async Task<bool> SaveChangesAsync()
        {
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateOrder(Order order)
        {
          
            _context.Entry(order).State = EntityState.Modified;
            await SaveChangesAsync();
            return true;
        }


        public async Task<bool> RemoveOrder(Order order)
        {
            _context.Orders.Remove(order);
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> OrderExists(int orderId)
        {
            return await _context.Orders.AnyAsync(e => e.Id == orderId);
        }
    
        public async Task<OrderViewModel?> GetOrderByIdAsync(int? orderId)
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

        public async Task<IEnumerable<Order>> GetOrders()
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

        public async Task<bool> AddOrder(OrderViewModel orderViewModel)
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

        public async Task<bool> UpdateOrder(OrderViewModel orderViewModel)
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

        public async Task<IEnumerable<Order>> GetRecentOrders()
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

        public Task<int> GetOrderCount()
        {
            return _context.Orders.CountAsync();
        }

        public Task<int> GetTodaySalesCount()
        {
            return _context.Orders
                .Where(o => o.OrderDate.Date == DateTime.Today)
                .CountAsync();
        }

        public Task<decimal> GetTodaySalesValue()
        {
            return _context.Orders
                .Where(o => o.OrderDate.Date == DateTime.Today)
                .SumAsync(o => o.TotalPrice);
        }

        public async Task<IEnumerable<Product>> GetTopSellingProducts()
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

        public async Task<IEnumerable<Product>> GetTopSellingProductsWithTotalQuantity()
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
        

        public async Task<IEnumerable<Product>> GetLowSellingProducts()
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

        public async Task<decimal> GetWeekSalesValue()
        {
            return await _context.Orders
                .Where(o => o.OrderDate.Date >= DateTime.Today.AddDays(-7))
                .SumAsync(o => o.TotalPrice);
        }

        public async Task<decimal> GetMonthSalesValue()
        {
            return await _context.Orders
                .Where(o => o.OrderDate.Date >= DateTime.Today.AddMonths(-1))
                .SumAsync(o => o.TotalPrice);
        }

       

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

        public async Task<List<decimal>> GetMonthlySalesList()
        {
            try{
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

        public async Task<List<decimal>> GetWeeklySalesList()
        {
            try{
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

        public async Task<List<decimal>> GetDailySalesList()
        {
            try{
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