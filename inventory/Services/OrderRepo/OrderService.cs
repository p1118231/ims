using inventory.Data;
using inventory.Models;
using inventory.Models.Orders;
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

        

        
    }
}