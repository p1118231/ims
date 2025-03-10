using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using inventory.Controllers;
using inventory.Models;
using inventory.Services.ProductRepo;
using inventory.Services.CategoryRepo;
using inventory.Services.SupplierRepo;
using inventory.Services.PriceOptimisation;
using inventory.Services.NotificationRepo;
using inventory.Services.StockOptimisationRepo;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Testing.Platform.Logging;

namespace inventoryTest.Controllers;

[TestClass]
public class ProductControllerTest
{
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<ProductController>> _loggerMock;
    private readonly Mock<IProductService> _productServiceMock;
    private readonly Mock<ICategoryService> _categoryServiceMock;
    private readonly Mock<ISupplierService> _supplierServiceMock;
    private readonly Mock<IPricePredictionService> _pricePredictionServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IStockOptimisationService> _stockOptimisationServiceMock;
    private readonly ProductController _controller;

    public ProductControllerTest()
    {
        _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<ProductController>>();
        _productServiceMock = new Mock<IProductService>();
        _categoryServiceMock = new Mock<ICategoryService>();
        _supplierServiceMock = new Mock<ISupplierService>();
        _pricePredictionServiceMock = new Mock<IPricePredictionService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _stockOptimisationServiceMock = new Mock<IStockOptimisationService>();

        _controller = new ProductController(
            _loggerMock.Object,
            _productServiceMock.Object,
            _categoryServiceMock.Object,
            _supplierServiceMock.Object,
            _pricePredictionServiceMock.Object,
            _notificationServiceMock.Object,
            _stockOptimisationServiceMock.Object
        );
    }

    

    // Test Index action with search query
    [TestMethod]
    [Fact]
    public async Task Index_WithQuery_FiltersProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Test Product" },
            new Product { ProductId = 2, Name = "Other Product" }
        };
         var categories = new List<Category> { new Category { CategoryId = 1, Name = "Test Category", Products = new List<Product>() } };
        var suppliers = new List<Supplier> { new Supplier { SupplierId = 1, Name = "Test Supplier", Products = new List<Product>() } };
        _productServiceMock.Setup(s => s.GetProducts()).ReturnsAsync(products);
        _categoryServiceMock.Setup(s => s.GetCategories()).ReturnsAsync(categories);
        _supplierServiceMock.Setup(s => s.GetSuppliers()).ReturnsAsync(suppliers);

        // Act
        var result = await _controller.Index("Test", null, null);

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        var model = Xunit.Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
        Xunit.Assert.Single(model); // Only "Test Product" should match
        Xunit.Assert.Equal("Test", viewResult.ViewData["SearchQuery"]);
    }

    // Test Index action with exception
    [TestMethod]
    [Fact]
    public async Task Index_Exception_ReturnsEmptyProducts()
    {
        // Arrange
        _productServiceMock.Setup(s => s.GetProducts()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Index(null, null, null);

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        var model = Xunit.Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
        Xunit.Assert.Empty(model);
        _loggerMock.Verify(
            x => x.Log(
                Microsoft.Extensions.Logging.LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failure to access product service")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once());
    }

    // Test Create GET
    [TestMethod]
    [Fact]
    public async Task Create_Get_ReturnsViewWithDropdowns()
    {
         var categories = new List<Category> { new Category { CategoryId = 1, Name = "Test Category", Products = new List<Product>() } };
        var suppliers = new List<Supplier> { new Supplier { SupplierId = 1, Name = "Test Supplier", Products = new List<Product>() } };
       
        _categoryServiceMock.Setup(s => s.GetCategories()).ReturnsAsync(categories);
        _supplierServiceMock.Setup(s => s.GetSuppliers()).ReturnsAsync(suppliers);

        // Act
        var result = await _controller.Create();

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.Equal(categories, viewResult.ViewData["Categories"]);
        Xunit.Assert.Equal(suppliers, viewResult.ViewData["Suppliers"]);
    }

    // Test Create POST with valid model
    [TestMethod]
    [Fact]
    public async Task Create_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "New Product", CategoryId = 1, SupplierId = 1 };
        _productServiceMock.Setup(s => s.AddProduct(product)).Returns(Task.FromResult(true));
        _productServiceMock.Setup(s => s.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(product);

        // Assert
        var redirectResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
        Xunit.Assert.Equal("Index", redirectResult.ActionName);
    }

    // Test Create POST with invalid model
    [TestMethod]
    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsView()
    {
        // Arrange
        var product = new Product { Name = "" }; // Invalid: Name is required
        _controller.ModelState.AddModelError("Name", "The Name field is required.");
         var categories = new List<Category> { new Category { CategoryId = 1, Name = "Test Category", Products = new List<Product>() } };
        var suppliers = new List<Supplier> { new Supplier { SupplierId = 1, Name = "Test Supplier", Products = new List<Product>() } };
       
        _categoryServiceMock.Setup(s => s.GetCategories()).ReturnsAsync(categories);
        _supplierServiceMock.Setup(s => s.GetSuppliers()).ReturnsAsync(suppliers);

        // Act
        var result = await _controller.Create(product);

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.Equal(product, viewResult.Model);
        Xunit.Assert.Equal(categories, viewResult.ViewData["Categories"]);
        Xunit.Assert.Equal(suppliers, viewResult.ViewData["Suppliers"]);
    }

    // Test Details with valid ID
    [TestMethod]
    [Fact]
    public async Task Details_ValidId_ReturnsViewWithProduct()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product" };
        _productServiceMock.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);
        _pricePredictionServiceMock.Setup(s => s.PredictPriceAsync(1)).ReturnsAsync(new PricePredictionResponse { predicted_price = 10.0 });
        _stockOptimisationServiceMock.Setup(s => s.PredictStockLevelAsync(1)).ReturnsAsync(new StockOptimisationResponse { predicted_stock_level = 100.0 });

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.Equal(product, viewResult.Model);
        Xunit.Assert.Equal(10.0, viewResult.ViewData["PredictedPrice"]);
        Xunit.Assert.Equal(100.0, viewResult.ViewData["PredictedStock"]);
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

    // Test EditField GET with valid ID and field
    [TestMethod]
    [Fact]
    public async Task EditField_Get_ValidIdAndField_ReturnsView()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Quantity = 50 };
        _productServiceMock.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _controller.EditField(1, "quantity");

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.Equal(product, viewResult.Model);
        Xunit.Assert.Equal("quantity", viewResult.ViewData["FieldToEdit"]);
        Xunit.Assert.Equal("50", viewResult.ViewData["FieldValue"]);
    }


    // Test Delete GET with valid ID
    [TestMethod]
    [Fact]
    public async Task Delete_Get_ValidId_ReturnsView()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product" };
        _productServiceMock.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        Xunit.Assert.Equal(product, viewResult.Model);
    }

    // Test Delete POST with valid ID
    [TestMethod]
    [Fact]
    public async Task Delete_Post_ValidId_RedirectsToIndex()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product" };
        _productServiceMock.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);
        _productServiceMock.Setup(s => s.RemoveProduct(product)).Returns(Task.FromResult(true));
        _productServiceMock.Setup(s => s.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirectResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
        Xunit.Assert.Equal("Index", redirectResult.ActionName);
    }
}