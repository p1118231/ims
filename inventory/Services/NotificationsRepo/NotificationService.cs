// NotificationService.cs
using inventory.Data;
using inventory.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace inventory.Services.NotificationRepo;
public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ProductContext _context;

    public NotificationService(IHubContext<NotificationHub> hubContext, ProductContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }

    
    public async Task<List<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications.OrderByDescending(n => n.Date).ToListAsync();
        }

        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id);
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            _context.Entry(notification).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkNotificationAsReadAsync(int id)
        {
            var notification = await GetNotificationByIdAsync(id);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await UpdateNotificationAsync(notification);
            }
        }

        public async Task<List<Notification>> GetAllUnreadNotificationsAsync()
        {
            return await _context.Notifications.Where(n => !n.IsRead).ToListAsync();
        }

        public async Task<int> GetAllUnreadNotificationCountAsync()
        {
            return await _context.Notifications.CountAsync(n => !n.IsRead);
        }

    
}
