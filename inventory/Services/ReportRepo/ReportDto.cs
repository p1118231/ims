using inventory.Models;
using inventory.Models.Orders;

namespace inventory.Services.ReportRepo
{
    public class ReportDto
    {
       
        public int TodaySalesCount { get; set; }
        public decimal TodaySalesValue { get; set; }

        public int WeekSalesCount { get; set; }
        public int MonthSalesCount { get; set; }

        public decimal WeekSalesValue { get; set; }
        public decimal MonthSalesValue { get; set; }


        public List<Product>? LowStockProducts { get; set; }
        
        
        public List<Product>? TopSellingProducts { get; set; }
        public List<Product>? LowSellingProducts { get; set; }

        public List<Supplier>? TopSuppliers { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalCategories { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }

        public decimal TotalSales{get; set;}

        
    }
}   