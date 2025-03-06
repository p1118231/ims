using inventory.Services.ReportRepo;
using Microsoft.AspNetCore.Mvc;

namespace inventory.Controllers;

public class ReportController : Controller
{
    private readonly ILogger<ReportController> _logger;
    private readonly IReportService _reportService;

    public ReportController(ILogger<ReportController> logger, IReportService reportService)
    {
        _logger = logger;
        _reportService = reportService;
    }
    
    public async Task<IActionResult> Index()
    {
        ReportDto report = null!;
        try
        {
            report = await _reportService.GetReport();
            return View(report);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"failure to access report service : {ex.Message}");
            return (IActionResult)(report = new ReportDto());
            
        }
    }
}