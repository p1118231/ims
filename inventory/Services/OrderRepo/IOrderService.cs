using inventory.Models;
using inventory.Models.Orders;
using inventory.Services.AnalyticsRepo;
using inventory.Services.AnalyticsResp;

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

    Task<decimal> GetWeekSalesValue();
    Task<decimal> GetMonthSalesValue();

    Task<List<decimal>> GetMonthlySalesList();
    Task<List<decimal>> GetWeeklySalesList();
    Task<List<decimal>> GetDailySalesList();

    Task<IEnumerable<Product>> GetTopSellingProducts();
    Task<IEnumerable<Product>> GetLowSellingProducts();

    Task<List<SalesTrendDto>> GetSalesTrend();
    Task<List<CategorySalesDto>> GetCategorySales();
    
}