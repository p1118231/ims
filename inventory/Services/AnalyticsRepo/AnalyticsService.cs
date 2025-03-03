using inventory.Services.CategoryRepo;
using inventory.Services.OrderRepo;
using inventory.Services.ProductRepo;
using inventory.Services.SalesPrediction;
using inventory.Services.SupplierRepo;

namespace inventory.Services.AnalyticsRepo;

public class AnalyticsService:IAnalyticsService{

    private readonly ILogger<AnalyticsService> _logger;
    private readonly IProductService _productService;
    private readonly IOrderService  _orderService;
    private readonly IForecastService   _forecastService;
    private readonly ICategoryService _categoryService;
    private readonly ISupplierService  _supplierService;



    public AnalyticsService(ILogger<AnalyticsService> logger, IProductService productService, IOrderService orderService, IForecastService forecastService, ICategoryService categoryService, ISupplierService supplierService)
    {
        _logger = logger;
        _productService = productService;
        _orderService = orderService;
        _forecastService = forecastService;
        _categoryService = categoryService;
        _supplierService = supplierService;
    }
    
    public async Task<AnalyticsDto> GetAnalytics()
    {
    var products = await _productService.GetProducts();
    var orders = await _orderService.GetOrders();
    var forecast = await _forecastService.GetTodaysSalesForecastAsync();
    var suppliers = await _supplierService.GetSuppliers();
    var categories = await _categoryService.GetCategories();

    var analyticsDto = new AnalyticsDto
    {
        TotalProducts = products.Count(),
        TotalOrders = orders.Count(),
        PredictedDailySales = forecast.daily_sales,
        PredictedWeeklySales = forecast.weekly_sales,   
        PredictedMonthlySales = forecast.monthly_sales,
        TotalSuppliers = suppliers.Count(),
        TotalCategories = categories.Count(),
        TodaySales = orders.Where(o => o.OrderDate.Date == DateTime.Today.Date).Sum(o => o.TotalPrice),
        WeekSales = orders.Where(o => o.OrderDate.Date >= DateTime.Today.Date.AddDays(-7)).Sum(o => o.TotalPrice),
        MonthSales = orders.Where(o => o.OrderDate.Date >= DateTime.Today.Date.AddMonths(-1)).Sum(o => o.TotalPrice),
        TotalSales = orders.Sum(o => o.TotalPrice),
        Days = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" },
        Months = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" },
        MonthlySales = orders.GroupBy(o => o.OrderDate.Month).Select(g => g.Sum(o => o.TotalPrice)).ToList(),
        WeeklySales = orders.GroupBy(o => o.OrderDate.DayOfWeek).Select(g => g.Sum(o => o.TotalPrice)).ToList(),
        DailySales = orders.GroupBy(o => o.OrderDate.Date).Select(g => g.Sum(o => o.TotalPrice)).ToList(),
        LowStockCount = products.Count(p => p.Quantity < 5),
        OutOfStockCount = products.Count(p => p.Quantity == 0),
        OverstockCount = products.Count(p => p.Quantity > 200)

    };

    return analyticsDto;
    }

}