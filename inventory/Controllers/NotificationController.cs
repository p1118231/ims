using inventory.Services.NotificationRepo;
using Microsoft.AspNetCore.Mvc;

namespace inventory.Controllers;
    
       
[ApiController]

[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var notifications = await _notificationService.GetAllUnreadNotificationsAsync();
        return Ok(notifications);
    }

    [HttpPost("markAsRead/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationService.MarkNotificationAsReadAsync(id);
        return Ok();
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _notificationService.DeleteNotificationAsync(id);
        return Ok();
    }
}
