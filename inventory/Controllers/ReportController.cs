using inventory.Services.ReportRepo;
using Microsoft.AspNetCore.Mvc;

namespace inventory.Controllers;

public class ReportController : Controller
{
    private readonly ILogger<ReportController> _logger;
    private readonly IReportService _reportService;

    // Constructor to initialize logger and report service
    public ReportController(ILogger<ReportController> logger, IReportService reportService)
    {
        _logger = logger;
        _reportService = reportService;
    }
    
    // Action method to get the report and display it
    public async Task<IActionResult> Index()
    {
        ReportDto report = null!;
        try
        {
            // Attempt to get the report from the report service
            report = await _reportService.GetReport();
            return View(report);
        }
        catch (Exception ex)
        {
            // Log a warning if there is an exception
            _logger.LogWarning($"Failure to access report service: {ex.Message}");
            // Return an empty report in case of an error
            return View(new ReportDto());
        }
    }
}