using inventory.Services.SalesPrediction;
using Microsoft.AspNetCore.Mvc;

namespace inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : Controller
    {
        private readonly IForecastService _forecastService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IForecastService forecastService, ILogger<DashboardController> logger)
        {
            _forecastService = forecastService;
            _logger = logger;
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
    }
}