using System.Diagnostics;
using inventory.Models;
using inventory.Services.DashboardRepo;
using inventory.Services.NotificationRepo;
using inventory.Services.SalesPrediction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventory.Controllers
{
    // HomeController handles the requests for the home page of the inventory system
    public class HomeController : Controller
    {
        private readonly IForecastService _forecastService;
        private readonly INotificationService _notificationService;
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<HomeController> _logger;

        // Constructor to initialize the services and logger
        public HomeController(IForecastService forecastService, ILogger<HomeController> logger, INotificationService notificationService, IDashboardService dashboardService)
        {
            _forecastService = forecastService;
            _logger = logger;
            _notificationService = notificationService;
            _dashboardService = dashboardService;
        }

        // Action method to handle GET requests for the dashboard index
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            HomeViewModel dashboard = null!;

            try
            {
                // Fetch the dashboard data
                dashboard = await _dashboardService.GetDashboardData();
                Console.WriteLine(dashboard);
            }
            catch (Exception ex)
            {
                // Handle any errors that occur while fetching the data
                dashboard = new HomeViewModel();
                _logger.LogError(ex, "An error occurred while fetching the sales dashboard data");
                return StatusCode(500, "An error occurred while fetching the sales dashboard data");
            }

            // Return the dashboard view with the fetched data
            return View(dashboard);
        }

        // Action method to handle errors
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}