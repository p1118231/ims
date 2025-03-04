using inventory.Services.AnalyticsRepo;
using Microsoft.AspNetCore.Mvc;

namespace inventory.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly ILogger<AnalyticsController> _logger;
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(ILogger<AnalyticsController> logger, IAnalyticsService analyticsService)
        {
            _logger = logger;
            _analyticsService = analyticsService;
        }
        public async Task<IActionResult> Index()
        {
            AnalyticsDto analytics = null!;
            try
            {
                analytics = await _analyticsService.GetAnalytics();
                return View(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"failure to access analytics service : {ex.Message}");
                return View("Error");
            }
        }

        public async Task<IActionResult> SalesTrends(){

            AnalyticsDto analytics = null!;
            List<SalesTrendDto> salesTrend = new List<SalesTrendDto>();
            try{
                analytics = await _analyticsService.GetAnalytics();
                salesTrend = analytics.SalesTrend ?? new List<SalesTrendDto>();
                return Json(salesTrend);
        }catch(Exception ex){
            _logger.LogWarning($"failure to access analytics service : {ex.Message}");
            return View("Error");
        }
        }
    }
}