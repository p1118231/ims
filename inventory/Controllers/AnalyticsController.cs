using inventory.Services.AnalyticsRepo;
using Microsoft.AspNetCore.Mvc;

namespace inventory.Controllers
{
    [Route("api/[controller]")]
    public class AnalyticsController : Controller
    {
        private readonly ILogger<AnalyticsController> _logger;
        private readonly IAnalyticsService _analyticsService;

        // Constructor to initialize logger and analytics service
        public AnalyticsController(ILogger<AnalyticsController> logger, IAnalyticsService analyticsService)
        {
            _logger = logger;
            _analyticsService = analyticsService;
        }

        // Action method to get analytics data and return it to the view
        public async Task<IActionResult> Index()
        {
            AnalyticsDto analytics = null!;
            try
            {
                // Attempt to get analytics data
                analytics = await _analyticsService.GetAnalytics();
                return View(analytics);
            }
            catch (Exception ex)
            {
                // Log warning and return error view in case of exception
                _logger.LogWarning($"Failure to access analytics service: {ex.Message}");
                return View("Error");
            }
        }

        // Action method to get sales trends data and return it as JSON
        [HttpGet("salestrends")]
        public async Task<IActionResult> SalesTrends()
        {
            AnalyticsDto analytics = null!;
            List<SalesTrendDto> salesTrend = new List<SalesTrendDto>();
            try
            {
                // Attempt to get analytics data
                analytics = await _analyticsService.GetAnalytics();
                salesTrend = analytics.SalesTrend ?? new List<SalesTrendDto>();
                return Json(salesTrend);
            }
            catch (Exception ex)
            {
                // Log warning and return error view in case of exception
                _logger.LogWarning($"Failure to access analytics service: {ex.Message}");
                return View("Error");
            }
        }
    }
}