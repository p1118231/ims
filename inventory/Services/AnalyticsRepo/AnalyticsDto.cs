using inventory.Models;
using inventory.Models.Orders;

namespace inventory.Services.AnalyticsRepo
{
    public class AnalyticsDto
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }

        public decimal TodaySales{get; set;}
        public decimal WeekSales { get; set; }
        public decimal MonthSales { get; set; }

        public int PredictedDailySales { get; set; }
        public int PredictedWeeklySales { get; set; }   
        public int PredictedMonthlySales { get; set; }

        public List<string>? Months { get; set; } // Add this property
        public List<string>? Days { get; set; } // Add this property

        public List<Product>? lowStockProducts { get; set; }
        public List<decimal>? MonthlySales { get; set; }
        public List<decimal>? WeeklySales { get; set; }
        public List<decimal>? DailySales { get; set; }

        public List<SalesTrendDto>? SalesTrend {get;set;}


    }
}