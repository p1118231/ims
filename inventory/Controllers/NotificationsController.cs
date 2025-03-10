using inventory.Services.NotificationRepo;
using Microsoft.AspNetCore.Mvc;

namespace inventory.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    // Constructor to initialize the notification service and logger
    public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    // GET: api/Notifications
    // Fetches all notifications and returns them to the view
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return View(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching notifications");
            return BadRequest(ex.Message);
        }
    }

    // POST: api/Notifications/MarkAsRead/{id}
    // Marks a notification as read by its ID
    [HttpPost("MarkAsRead/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            await _notificationService.MarkNotificationAsReadAsync(id);
            Console.WriteLine("Notification marked as read");
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while marking notification as read");
            return BadRequest(ex.Message);
        }
    }

    // POST: api/Notifications/Delete/{id}
    // Deletes a notification by its ID
    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _notificationService.DeleteNotificationAsync(id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting notification");
            return BadRequest(ex.Message);
        }
    }

    // GET: api/Notifications/count
    // Fetches the count of all unread notifications
    [HttpGet("count")]
    public async Task<IActionResult> Count()
    {
        try
        {
            var count = await _notificationService.GetAllUnreadNotificationCountAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching notification count");
            return BadRequest(ex.Message);
        }
    }
}
