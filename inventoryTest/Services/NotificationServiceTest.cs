using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using inventory.Services.NotificationRepo;
using inventory.Data;
using inventory.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace inventoryTest.Services;

public class NotificationServiceTest
{
    private readonly ProductContext _context;
    private readonly Mock<IHubContext<NotificationHub>> _hubContextMock;
    private readonly NotificationService _service;

    public NotificationServiceTest()
    {
        // Set up in-memory database
        var options = new DbContextOptionsBuilder<ProductContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;
        _context = new ProductContext(options);

        _hubContextMock = new Mock<IHubContext<NotificationHub>>();

        _service = new NotificationService(_hubContextMock.Object, _context);

        // Seed initial data
        SeedData();
    }

    private void SeedData()
    {
        _context.Notifications.AddRange(
            new Notification { NotificationId = 1, Message = "Test1", Date = DateTime.Now, IsRead = false },
            new Notification { NotificationId = 2, Message = "Test2", Date = DateTime.Now.AddHours(-1), IsRead = true }
        );
        _context.SaveChanges();
    }

    // Clean up after each test
    [Fact]
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllNotificationsAsync_Success_ReturnsOrderedList()
    {
        // Act
        var result = await _service.GetAllNotificationsAsync();

        // Assert
        var notifications = result.ToList();
        Xunit.Assert.Equal(2, notifications.Count);
        Xunit.Assert.Equal("Test1", notifications[0].Message); // Most recent first
        Xunit.Assert.Equal("Test2", notifications[1].Message);
    }

    [Fact]
    public async Task GetAllNotificationsAsync_Exception_Throws()
    {
        // Arrange
        var badContext = CreateBadContext();
        var badService = new NotificationService(_hubContextMock.Object, badContext);

        // Act & Assert
        var exception = await Xunit.Assert.ThrowsAsync<Exception>(() => badService.GetAllNotificationsAsync());
        Xunit.Assert.Equal("Error retrieving all notifications", exception.Message);
    }

    [Fact]
    public async Task GetNotificationByIdAsync_Success_ReturnsNotification()
    {
        // Act
        var result = await _service.GetNotificationByIdAsync(1);

        // Assert
        Xunit.Assert.NotNull(result);
        Xunit.Assert.Equal(1, result.NotificationId);
        Xunit.Assert.Equal("Test1", result.Message);
    }

    [Fact]
    public async Task GetNotificationByIdAsync_NonExistent_ReturnsNull()
    {
        // Act
        var result = await _service.GetNotificationByIdAsync(999);

        // Assert
        Xunit.Assert.Null(result);
    }

    [Fact]
    public async Task CreateNotificationAsync_Success_AddsNotification()
    {
        // Arrange
        var notification = new Notification { NotificationId = 3, Message = "Test3", Date = DateTime.Now };

        // Act
        await _service.CreateNotificationAsync(notification);

        // Assert
        var added = await _context.Notifications.FindAsync(3);
        Xunit.Assert.NotNull(added);
        Xunit.Assert.Equal("Test3", added.Message);
    }

    [Fact]
    public async Task CreateNotificationAsync_Exception_Throws()
    {
        // Arrange
        var badContext = CreateBadContext();
        var badService = new NotificationService(_hubContextMock.Object, badContext);
        var notification = new Notification { NotificationId = 1, Message = "Duplicate" }; // Duplicate ID

        // Act & Assert
        var exception = await Xunit.Assert.ThrowsAsync<Exception>(() => badService.CreateNotificationAsync(notification));
        Xunit.Assert.Equal("Error creating notification", exception.Message);
    }

    [Fact]
    public async Task UpdateNotificationAsync_Success_UpdatesNotification()
    {
        // Arrange
        var notification = await _context.Notifications.FindAsync(1);
        notification.Message = "Updated";

        // Act
        await _service.UpdateNotificationAsync(notification);

        // Assert
        var updated = await _context.Notifications.FindAsync(1);
        Xunit.Assert.Equal("Updated", updated.Message);
    }

    [Fact]
    public async Task DeleteNotificationAsync_Success_RemovesNotification()
    {
        // Act
        await _service.DeleteNotificationAsync(1);

        // Assert
        var deleted = await _context.Notifications.FindAsync(1);
        Xunit.Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteNotificationAsync_NonExistent_DoesNothing()
    {
        // Act
        await _service.DeleteNotificationAsync(999);

        // Assert
        var countBefore = await _context.Notifications.CountAsync();
        Xunit.Assert.Equal(2, countBefore); // No change
    }

    [Fact]
    public async Task MarkNotificationAsReadAsync_Unread_MarksAsRead()
    {
        // Act
        await _service.MarkNotificationAsReadAsync(1);

        // Assert
        var notification = await _context.Notifications.FindAsync(1);
        Xunit.Assert.True(notification.IsRead);
    }

    [Fact]
    public async Task MarkNotificationAsReadAsync_AlreadyRead_DoesNothing()
    {
        // Act
        await _service.MarkNotificationAsReadAsync(2);

        // Assert
        var notification = await _context.Notifications.FindAsync(2);
        Xunit.Assert.True(notification.IsRead); // Still true, no unnecessary update
    }

    [Fact]
    public async Task GetAllUnreadNotificationsAsync_Success_ReturnsUnread()
    {
        // Act
        var result = await _service.GetAllUnreadNotificationsAsync();

        // Assert
        var notifications = result.ToList();
        Xunit.Assert.Single(notifications);
        Xunit.Assert.Equal(1, notifications[0].NotificationId);
    }

    [Fact]
    public async Task GetAllUnreadNotificationCountAsync_Success_ReturnsCount()
    {
        // Act
        var result = await _service.GetAllUnreadNotificationCountAsync();

        // Assert
        Xunit.Assert.Equal(1, result); // Only ID 1 is unread
    }

    [Fact]
    public async Task GetAllUnreadNotificationCountAsync_Exception_Throws()
    {
        // Arrange
        var badContext = CreateBadContext();
        var badService = new NotificationService(_hubContextMock.Object, badContext);

        // Act & Assert
        var exception = await Xunit.Assert.ThrowsAsync<Exception>(() => badService.GetAllUnreadNotificationCountAsync());
        Xunit.Assert.Equal("Error retrieving unread notification count", exception.Message);
    }

    // Helper to create a bad context that throws exceptions
    private ProductContext CreateBadContext()
    {
        var options = new DbContextOptionsBuilder<ProductContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var badContext = new Mock<ProductContext>(options);
        badContext.Setup(c => c.Notifications).Throws(new Exception("DB error"));
        return badContext.Object;
    }
}