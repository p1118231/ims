using inventory.Data;
using inventory.Services.OrderRepo;
using inventory.Services.ProductRepo;
using inventory.Services.SalesPrediction;

namespace inventory.Services.DashboardRepo
{
    public class DashboardService : IDashboardService
    {
        private readonly ProductContext _context;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        private readonly IForecastService _forecastService;

        public DashboardService(ProductContext context, IProductService productService, IOrderService orderService, IForecastService forecastService)
        {
            _context = context;
            _productService = productService;
            _orderService = orderService;
            _forecastService = forecastService;
        }

        public async Task<HomeViewModel> GetDashboardData()
        {
            //SalesForecastResponse forecastResponse = null!;

           // forecastResponse = await _forecastService.GetTodaysSalesForecastAsync();

            var homeViewModel = new HomeViewModel();

            homeViewModel.inventoryCount = await _productService.GetProductCount();
            homeViewModel.todaySalesCount = await _orderService.GetTodaySalesCount();
            homeViewModel.todaySalesValue = await _orderService.GetTodaySalesValue();
            homeViewModel.lowStockProducts = (await _productService.GetLowStockProducts()).ToList();
            homeViewModel.recentOrders = (await _orderService.GetRecentOrders()).ToList();
            

            return homeViewModel;
        }
    }
}