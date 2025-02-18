using Microsoft.AspNetCore.SignalR;
namespace inventory.Services.NotificationRepo; 

  
public class NotificationHub : DynamicHub
{
    public async Task SendNotification(string message, string type)
    {
        await Clients.All.SendAsync("ReceiveNotification",  new { Type = type, Message = message });
    }
}