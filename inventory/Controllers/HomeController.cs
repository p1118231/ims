using System.Diagnostics;
using inventory.Models;
using inventory.Services.NotificationRepo;
using inventory.Services.SalesPrediction;
using Microsoft.AspNetCore.Mvc;

namespace inventory.Controllers
{
    
   // [ApiController]
    public class HomeController : Controller
    {
        private readonly IForecastService _forecastService;

        private readonly INotificationService _notificationService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IForecastService forecastService, ILogger<HomeController> logger, INotificationService notificationService)
        {
            _forecastService = forecastService;
            _logger = logger;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            SalesForecastResponse forecast = null!;
            try
            {
               
                forecast = await _forecastService.GetTodaysSalesForecastAsync();
                
            }
            catch (Exception ex)
            {
                forecast = new SalesForecastResponse();
                _logger.LogError(ex, "An error occurred while fetching the sales forecast");
                return StatusCode(500, "An error occurred while fetching the sales forecast");
                
            }

            return View(forecast);
           
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        }
}