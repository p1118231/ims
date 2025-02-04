using inventory.Models.Orders;

namespace inventory.Services.OrderRepo;

public interface IOrderService
{
    Task <IEnumerable<OrderViewModel>> GetOrders();
    Task <OrderViewModel?> GetOrderByIdAsync(int? id);
    Task <bool> SaveChangesAsync();
    Task<bool> UpdateOrder(OrderViewModel order);
    Task<bool> AddOrder(OrderViewModel order);
    Task<bool> RemoveOrder(Order order);
    Task <bool> OrderExists(int id);
}