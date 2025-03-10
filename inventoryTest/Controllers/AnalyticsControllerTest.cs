using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using inventory.Controllers;
using inventory.Services.AnalyticsRepo;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace inventoryTests.Controllers;

[TestClass]
public class AnalyticsControllerTest
{
    private readonly Mock<IAnalyticsService> _analyticsServiceMock;
    private readonly Mock<ILogger<AnalyticsController>> _loggerMock;
    private readonly AnalyticsController _controller;

    public AnalyticsControllerTest()
    {
        _analyticsServiceMock = new Mock<IAnalyticsService>();
        _loggerMock = new Mock<ILogger<AnalyticsController>>();

        _controller = new AnalyticsController(_loggerMock.Object, _analyticsServiceMock.Object);
    }

    // Test Index action with successful data retrieval
    [TestMethod]
    [Fact]
    public async Task Index_Success_ReturnsViewWithAnalytics()
    {
        // Arrange
        var analytics = new AnalyticsDto
        {
            TodaySales = 100m,
            SalesTrend = new List<SalesTrendDto> { new SalesTrendDto { Date = "2025-03-10", TotalSales = 100m } }
        };
        _analyticsServiceMock.Setup(s => s.GetAnalytics()).ReturnsAsync(analytics);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        var model = Xunit.Assert.IsType<AnalyticsDto>(viewResult.Model);
        Xunit.Assert.Equal(analytics, model);
    }

    // Test Index action with exception
    [TestMethod]
    [Fact]
    public async Task Index_Exception_ReturnsErrorView()
    {
        // Arrange
        _analyticsServiceMock.Setup(s => s.GetAnalytics()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.Equal("Error", viewResult.ViewName);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failure to access analytics service")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once());
    }

    // Test SalesTrends action with successful data retrieval
    [TestMethod]
    [Fact]
    public async Task SalesTrends_Success_ReturnsJsonWithSalesTrend()
    {
        // Arrange
        var analytics = new AnalyticsDto
        {
            SalesTrend = new List<SalesTrendDto>
            {
                new SalesTrendDto { Date = "2025-03-10", TotalSales = 100m },
                new SalesTrendDto { Date = "2025-03-11", TotalSales = 150m }
            }
        };
        _analyticsServiceMock.Setup(s => s.GetAnalytics()).ReturnsAsync(analytics);

        // Act
        var result = await _controller.SalesTrends();

        // Assert
        var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
        var salesTrend = Xunit.Assert.IsAssignableFrom<List<SalesTrendDto>>(jsonResult.Value);
       Xunit.Assert.Equal(2, salesTrend.Count);
        Xunit.Assert.Equal("2025-03-10", salesTrend[0].Date);
        Xunit.Assert.Equal(100m, salesTrend[0].TotalSales);
    }

    // Test SalesTrends action with null SalesTrend
    [TestMethod]
    [Fact]
    public async Task SalesTrends_NullSalesTrend_ReturnsEmptyJson()
    {
        // Arrange
        var analytics = new AnalyticsDto { SalesTrend = null };
        _analyticsServiceMock.Setup(s => s.GetAnalytics()).ReturnsAsync(analytics);

        // Act
        var result = await _controller.SalesTrends();

        // Assert
        var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
        var salesTrend = Xunit.Assert.IsAssignableFrom<List<SalesTrendDto>>(jsonResult.Value);
        Xunit.Assert.Empty(salesTrend);
    }

    // Test SalesTrends action with exception
    [TestMethod]
    [Fact]
    public async Task SalesTrends_Exception_ReturnsErrorView()
    {
        // Arrange
        _analyticsServiceMock.Setup(s => s.GetAnalytics()).ThrowsAsync(new Exception("Service failure"));

        // Act
        var result = await _controller.SalesTrends();

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.Equal("Error", viewResult.ViewName);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failure to access analytics service")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once());
    }
}