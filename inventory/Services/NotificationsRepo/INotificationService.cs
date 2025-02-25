using inventory.Models;

namespace inventory.Services.NotificationRepo;

public interface INotificationService
{
    Task SendNotificationAsync(string message, string type);

        Task<List<Notification>> GetAllNotificationsAsync();
        Task<Notification> GetNotificationByIdAsync(int id);
        Task CreateNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(int id);
        Task MarkNotificationAsReadAsync(int id);

        Task<List<Notification>> GetAllUnreadNotificationsAsync();
    
}
