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
        TotalSales = orders.Sum(o => o.TotalPrice),
        Days = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" },
        Months = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" },
      

    };
    analyticsDto.TodaySales= await _orderService.GetTodaySalesValue();
    analyticsDto.WeekSales= await _orderService.GetWeekSalesValue();
    analyticsDto.MonthSales= await _orderService.GetMonthSalesValue();
    analyticsDto.MonthlySales = await _orderService.GetMonthlySalesList();
    analyticsDto.WeeklySales = await _orderService.GetWeeklySalesList();
    analyticsDto.DailySales = await _orderService.GetDailySalesList();
    analyticsDto.SalesTrend = await _orderService.GetSalesTrend();
    analyticsDto.CategorySales = await _orderService.GetCategorySales();


    return analyticsDto;
    }

}