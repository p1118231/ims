using inventory.Services.CategoryRepo;
using inventory.Services.OrderRepo;
using inventory.Services.ProductRepo;
using inventory.Services.SalesPrediction;
using inventory.Services.SupplierRepo;

namespace inventory.Services.ReportRepo;

public class ReportService : IReportService
{
    private readonly ILogger _logger;
    private readonly IProductService    _productService;
    private readonly IOrderService _orderService;   
    private readonly IForecastService _forecastService;
    private readonly ISupplierService   _supplierService;

    private readonly ICategoryService _categoryService;


    public ReportService(ILogger<ReportService> logger, IProductService productService, IOrderService orderService, IForecastService forecastService, ISupplierService supplierService, ICategoryService categoryService)
    {
        _logger = logger;
        _productService = productService;
        _orderService = orderService;
        _forecastService = forecastService;
        _supplierService = supplierService;
        _categoryService = categoryService;
    }
       
    

    public async Task<ReportDto> GetReport()
    {
       try{
        
        var products = await _productService.GetProducts();
        var orders = await _orderService.GetOrders();
        var forecast = await _forecastService.GetTodaysSalesForecastAsync();
        var suppliers = await _supplierService.GetSuppliers();
        var categories = await _categoryService.GetCategories();

        var reportDto = new ReportDto
        {
            TotalProducts = products.Count(),
            TotalOrders = orders.Count(),
            TotalSuppliers = suppliers.Count(),
            TotalCategories = categories.Count(),
            TodaySalesCount = orders.Where(o => o.OrderDate.Date == DateTime.Today.Date).Count(),
            WeekSalesCount = orders.Where(o => o.OrderDate.Date >= DateTime.Today.Date.AddDays(-7)).Count(),
            MonthSalesCount = orders.Where(o => o.OrderDate.Date >= DateTime.Today.Date.AddMonths(-1)).Count(),
            TodaySalesValue = orders.Where(o => o.OrderDate.Date == DateTime.Today.Date).Sum(o => o.TotalPrice),
            WeekSalesValue = orders.Where(o => o.OrderDate.Date >= DateTime.Today.Date.AddDays(-7)).Sum(o => o.TotalPrice),
            MonthSalesValue = orders.Where(o => o.OrderDate.Date >= DateTime.Today.Date.AddMonths(-1)).Sum(o => o.TotalPrice),
            LowStockProducts = (await _productService.GetLowStockProducts()).ToList(),
            TopSellingProducts = (await _orderService.GetTopSellingProducts()).ToList(),
            LowSellingProducts = (await _orderService.GetLowSellingProducts()).ToList(),
            TopSuppliers = (await _supplierService.GetTopSuppliers()).ToList(),
            
            TotalSales = orders.Sum(o => o.TotalPrice)


        };
        return reportDto;
    

       }
       catch(Exception ex){
           _logger.LogWarning($"failure to access report service : {ex.Message}");
           return null!;
       }
    }
}