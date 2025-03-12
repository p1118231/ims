using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using inventory.Services.ProductRepo;
using inventory.Models;
using inventory.Data;
using inventory.Services.NotificationRepo;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using inventory.Services.StockOptimisationRepo;

namespace inventoryTest.Services;

[TestClass]
public class ProductServiceTest
{
    private readonly ProductContext _context;
    private readonly Mock<INotificationService> _notificationServiceMock;

    private readonly Mock<IStockOptimisationService> _stockOptimisationServiceMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _service;

    public ProductServiceTest()
    {
        // Set up in-memory database
        var options = new DbContextOptionsBuilder<ProductContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;
        _context = new ProductContext(options);

        _notificationServiceMock = new Mock<INotificationService>();
        _stockOptimisationServiceMock = new Mock<IStockOptimisationService>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        _service = new ProductService(_context, _notificationServiceMock.Object, _loggerMock.Object, _stockOptimisationServiceMock.Object);

        // Seed initial data
        SeedData();
    }
    
    [Fact]
    private void SeedData()
    {
        _context.Product.AddRange(
            new Product { ProductId = 1, Name = "Product1", Quantity = 50, CategoryId = 1, SupplierId = 1 },
            new Product { ProductId = 2, Name = "Product2", Quantity = 150, CategoryId = 2, SupplierId = 2 }
        );
        _context.SaveChanges();
    }
    [TestMethod]
    // Clean up after each test
    [Fact]
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    [TestMethod]
    [Fact]
    public async Task AddProduct_Success_ReturnsTrue()
    {
        // Arrange
        var product = new Product { ProductId = 3, Name = "Product3", Quantity = 200 };

        // Act
        var result = await _service.AddProduct(product);

        // Assert
        Xunit.Assert.True(result);
        var addedProduct = await _context.Product.FindAsync(3);
        Xunit.Assert.NotNull(addedProduct);
        Xunit.Assert.Equal("Product3", addedProduct.Name);
    }
    [TestMethod]
    [Fact]
    public async Task AddProduct_Exception_ReturnsFalse()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Duplicate" }; // Duplicate ID will cause exception

        // Act
        var result = await _service.AddProduct(product);

        // Assert
        Xunit.Assert.False(result);
        _loggerMock.VerifyLog(LogLevel.Error, "Error adding product", Times.Once());
    }
    
    [TestMethod]
    [Fact]
    public async Task GetProductByIdAsync_NonExistent_ReturnsNull()
    {
        // Act
        var result = await _service.GetProductByIdAsync(999);

        // Assert
        Xunit.Assert.Null(result);
    }
    
   
    [TestMethod]
    [Fact]
    public void ProductExists_Exists_ReturnsTrue()
    {
        // Act
        var result = _service.ProductExists(1);

        // Assert
        Xunit.Assert.True(result);
    }
    [TestMethod]
    [Fact]
    public void ProductExists_NotExists_ReturnsFalse()
    {
        // Act
        var result = _service.ProductExists(999);

        // Assert
        Xunit.Assert.False(result);
    }

    [TestMethod]
    [Fact]
    public async Task SaveChangesAsync_Success_Completes()
    {
        // Arrange
        var product = new Product { ProductId = 3, Name = "Product3" };
        await _context.Product.AddAsync(product);

        // Act
        await _service.SaveChangesAsync();

        // Assert
        Xunit.Assert.NotNull(await _context.Product.FindAsync(3));
    }
    [TestMethod]
    [Fact]
    public async Task UpdateProduct_Success_ReturnsTrue()
    {
        // Arrange
        var product = await _context.Product.FindAsync(1);
        product.Name = "UpdatedProduct";

        // Act
        var result = await _service.UpdateProduct(product);

        // Assert
        Xunit.Assert.True(result);
        var updatedProduct = await _context.Product.FindAsync(1);
        Xunit.Assert.Equal("UpdatedProduct", updatedProduct.Name);
    }
    [TestMethod]
    [Fact]
    public async Task CheckAndRestockProduct_LowStock_RestocksAndNotifies()
    {
        // Arrange
        _notificationServiceMock.Setup(s => s.CreateNotificationAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);

        // Act
        await _service.CheckAndRestockProduct();

        // Assert
        var product = await _context.Product.FindAsync(1);
        Xunit.Assert.Equal(150, product.Quantity); // 50 + 100
        _notificationServiceMock.Verify(
            s => s.CreateNotificationAsync(It.Is<Notification>(n => n.Message.Contains("Product1 has been restocked to 150"))),
            Times.Once());
        var product2 = await _context.Product.FindAsync(2);
        Xunit.Assert.Equal(150, product2.Quantity); // No change, above threshold
    }
    [TestMethod]
    [Fact]
    public async Task GetLowStockProducts_Success_ReturnsLowStock()
    {
        // Act
        var result = await _service.GetLowStockProducts();

        // Assert
        var products = result.ToList();
        Xunit.Assert.Single(products);
        Xunit.Assert.Equal(1, products[0].ProductId); // Only Product1 (50) is < 120
    }
    [TestMethod]
    [Fact]
    public async Task GetProductCount_Success_ReturnsCount()
    {
        // Act
        var result = await _service.GetProductCount();

        // Assert
        Xunit.Assert.Equal(2, result);
    }
    
}

// Helper to verify logger calls

public static class MoqExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel level, string messageContains, Times times)
    {
        loggerMock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(messageContains)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }
}