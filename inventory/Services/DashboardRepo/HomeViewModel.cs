using inventory.Models;
using inventory.Models.Orders;

namespace inventory.Services.DashboardRepo
{
    public class HomeViewModel
    {
        public int inventoryCount { get; set; }
        public int todaySalesCount { get; set; }
        public decimal todaySalesValue { get; set; }
       
        public int daily_sales { get; set; }    
        public int weekly_sales { get; set; }
        public int monthly_sales { get; set; }
        public  List<Product>? lowStockProducts { get; set; }
        public List<Order>? recentOrders { get; set; }

    }
}