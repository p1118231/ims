namespace inventory.Services.NotificationRepo;

public interface INotificationService
{
    Task SendNotificationAsync(string message, string type);
}