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

    // Get all notifications ordered by date in descending order
    public async Task<List<Notification>> GetAllNotificationsAsync()
    {
        try
        {
            return await _context.Notifications.OrderByDescending(n => n.Date).ToListAsync();
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception("Error retrieving all notifications", ex);
        }
    }

    // Get a notification by its ID
    public async Task<Notification> GetNotificationByIdAsync(int id)
    {
        try
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id);
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception($"Error retrieving notification with ID {id}", ex);
        }
    }

    // Create a new notification
    public async Task CreateNotificationAsync(Notification notification)
    {
        try
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception("Error creating notification", ex);
        }
    }

    // Update an existing notification
    public async Task UpdateNotificationAsync(Notification notification)
    {
        try
        {
            _context.Entry(notification).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception("Error updating notification", ex);
        }
    }

    // Delete a notification by its ID
    public async Task DeleteNotificationAsync(int id)
    {
        try
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception($"Error deleting notification with ID {id}", ex);
        }
    }

    // Mark a notification as read by its ID
    public async Task MarkNotificationAsReadAsync(int id)
    {
        try
        {
            var notification = await GetNotificationByIdAsync(id);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await UpdateNotificationAsync(notification);
            }
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception($"Error marking notification with ID {id} as read", ex);
        }
    }

    // Get all unread notifications
    public async Task<List<Notification>> GetAllUnreadNotificationsAsync()
    {
        try
        {
            return await _context.Notifications.Where(n => !n.IsRead).ToListAsync();
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception("Error retrieving unread notifications", ex);
        }
    }

    // Get the count of all unread notifications
    public async Task<int> GetAllUnreadNotificationCountAsync()
    {
        try
        {
            return await _context.Notifications.CountAsync(n => !n.IsRead);
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception("Error retrieving unread notification count", ex);
        }
    }
}
