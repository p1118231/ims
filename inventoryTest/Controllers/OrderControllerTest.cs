using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using inventory.Controllers;
using inventory.Models.Orders;
using inventory.Services.OrderRepo;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace inventoryTest.Controllers;

[TestClass]
public class OrderControllerTest
{
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly Mock<ILogger<OrderController>> _loggerMock;
    private readonly OrderController _controller;

    public OrderControllerTest()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _loggerMock = new Mock<ILogger<OrderController>>();

        _controller = new OrderController(_orderServiceMock.Object, _loggerMock.Object);
    }

    // Test Index action with no filters
    [TestMethod]
    [Fact]
    public async Task Index_NoFilters_ReturnsViewWithAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = 1, OrderDate = DateTime.Now },
            new Order { Id = 2, OrderDate = DateTime.Now.AddDays(-1) }
        };
        _orderServiceMock.Setup(s => s.GetOrders()).ReturnsAsync(orders);

        // Act
        var result = await _controller.Index(null, null, null);

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        var model = Xunit.Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.Model);
        Xunit.Assert.Equal(orders, model);
        Xunit.Assert.Null(viewResult.ViewData["SearchQuery"]);
        Xunit.Assert.Null(viewResult.ViewData["StartDate"]);
        Xunit.Assert.Null(viewResult.ViewData["EndDate"]);
    }

    // Test Index action with search query
    [TestMethod]
    [Fact]
    public async Task Index_WithQuery_FiltersOrdersById()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = 123, OrderDate = DateTime.Now },
            new Order { Id = 456, OrderDate = DateTime.Now.AddDays(-1) }
        };
        _orderServiceMock.Setup(s => s.GetOrders()).ReturnsAsync(orders);

        // Act
        var result = await _controller.Index("12", null, null);

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        var model = Xunit.Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.Model);
        Xunit.Assert.Single(model);
        Xunit.Assert.Equal(123, model.First().Id); // Only ID 123 contains "12"
        Xunit.Assert.Equal("12", viewResult.ViewData["SearchQuery"]);
    }

    // Test Index action with date range
    [TestMethod]
    [Fact]
    public async Task Index_WithDateRange_FiltersOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = 1, OrderDate = new DateTime(2025, 3, 10) },
            new Order { Id = 2, OrderDate = new DateTime(2025, 3, 11) },
            new Order { Id = 3, OrderDate = new DateTime(2025, 3, 12) }
        };
        _orderServiceMock.Setup(s => s.GetOrders()).ReturnsAsync(orders);
        var startDate = new DateTime(2025, 3, 10);
        var endDate = new DateTime(2025, 3, 11);

        // Act
        var result = await _controller.Index(null, startDate, endDate);

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        var model = Xunit.Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.Model);
        Xunit.Assert.Equal(2, model.Count()); // Orders from 3/10 and 3/11
        Xunit.Assert.Contains(model, o => o.Id == 1);
        Xunit.Assert.Contains(model, o => o.Id == 2);
        Xunit.Assert.DoesNotContain(model, o => o.Id == 3);
        Xunit.Assert.Equal("2025-03-10", viewResult.ViewData["StartDate"]);
        Xunit.Assert.Equal("2025-03-11", viewResult.ViewData["EndDate"]);
    }

    // Test Index action with only start date
    [TestMethod]
    [Fact]
    public async Task Index_WithStartDateOnly_FiltersOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = 1, OrderDate = new DateTime(2025, 3, 9) },
            new Order { Id = 2, OrderDate = new DateTime(2025, 3, 10) },
            new Order { Id = 3, OrderDate = new DateTime(2025, 3, 11) }
        };
        _orderServiceMock.Setup(s => s.GetOrders()).ReturnsAsync(orders);
        var startDate = new DateTime(2025, 3, 10);

        // Act
        var result = await _controller.Index(null, startDate, null);

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        var model = Xunit.Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.Model);
        Xunit.Assert.Equal(2, model.Count()); // Orders from 3/10 and 3/11
        Xunit.Assert.Contains(model, o => o.Id == 2);
        Xunit.Assert.Contains(model, o => o.Id == 3);
        Xunit.Assert.Equal("2025-03-10", viewResult.ViewData["StartDate"]);
        Xunit.Assert.Null(viewResult.ViewData["EndDate"]);
    }

    // Test Index action with exception
    [TestMethod]
    [Fact]
    public async Task Index_Exception_ReturnsEmptyOrders()
    {
        // Arrange
        _orderServiceMock.Setup(s => s.GetOrders()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Index(null, null, null);

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        var model = Xunit.Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.Model);
        Xunit.Assert.Empty(model);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failure to access order service")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once());
    }

  

    // Test Details with null ID
    [TestMethod]
    [Fact]
    public async Task Details_NullId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.Details(null);

        // Assert
        Xunit.Assert.IsType<NotFoundResult>(result);
    }

    // Test Details with non-existent ID
    [TestMethod]
    [Fact]
    public async Task Details_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(999)).ReturnsAsync((OrderViewModel?)null);

        // Act
        var result = await _controller.Details(999);

        // Assert
        Xunit.Assert.IsType<NotFoundResult>(result);
    }

    // Test Details with exception
    [TestMethod]
    [Fact]
    public async Task Details_Exception_ReturnsInternalServerError()
    {
        // Arrange
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1)).ThrowsAsync(new Exception("Service failure"));

        // Act
        var result = await _controller.Details(1);

        // Assert
        var statusCodeResult = Xunit.Assert.IsType<ObjectResult>(result);
        Xunit.Assert.Equal(500, statusCodeResult.StatusCode);
        Xunit.Assert.Equal("Internal server error", statusCodeResult.Value);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains("Failure to access order details")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once());
    }
}