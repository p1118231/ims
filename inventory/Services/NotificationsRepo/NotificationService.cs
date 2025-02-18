// NotificationService.cs
using Microsoft.AspNetCore.SignalR;

namespace inventory.Services.NotificationRepo;
public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(string type, string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", new { Type = type, Message = message });
    }
}
