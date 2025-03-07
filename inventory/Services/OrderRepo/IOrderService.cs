using inventory.Models;
using inventory.Models.Orders;

namespace inventory.Services.OrderRepo;

public interface IOrderService
{
    Task <IEnumerable<Order>> GetOrders();
    Task <OrderViewModel?> GetOrderByIdAsync(int? id);
    Task <bool> SaveChangesAsync();
    Task<bool> UpdateOrder(OrderViewModel order);
    Task<bool> AddOrder(OrderViewModel order);
    Task<bool> RemoveOrder(Order order);
    Task <bool> OrderExists(int id);

    Task<IEnumerable<Order>> GetRecentOrders();
    Task<int> GetOrderCount();
    Task<int> GetTodaySalesCount();
    Task<decimal> GetTodaySalesValue();

    Task<IEnumerable<Product>> GetTopSellingProducts();
    Task<IEnumerable<Product>> GetLowSellingProducts();
    
}