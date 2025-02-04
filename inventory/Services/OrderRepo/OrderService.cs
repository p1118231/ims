using inventory.Data;
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
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Product?.Price ?? 0
                }).ToList()
            };
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems!)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            return orders.Select(o => new OrderViewModel
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                Items = o.OrderItems?.Select(oi => new OrderItemViewModel
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
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
                var orderItemViewModel = orderViewModel.Items?.FirstOrDefault(oi => oi.OrderItemId == orderItem.OrderItemId);

                if (orderItemViewModel == null)
                {
                    _logger.LogError($"OrderItem with id {orderItem.OrderItemId} not found");
                    return false;
                }

                orderItem.Quantity = orderItemViewModel.Quantity;
                orderItem.Price = orderItemViewModel.Price;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }}