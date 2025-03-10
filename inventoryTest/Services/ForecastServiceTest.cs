using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using inventory.Services.SalesPrediction;
using RichardSzalay.MockHttp;
using System.Text;

namespace inventory.Tests.Services;

public class ForecastServiceTest
{
    private readonly Mock<ILogger<ForecastService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly MockHttpMessageHandler _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly ForecastService _service;

    public ForecastServiceTest()
    {
        _loggerMock = new Mock<ILogger<ForecastService>>();
        _configurationMock = new Mock<IConfiguration>();
        _httpMessageHandlerMock = new MockHttpMessageHandler();

        // Set up configuration to return a base URL
        _configurationMock.Setup(c => c["PredictionService:BaseUrl"]).Returns("http://mockapi.com");

        // Create HttpClient with mocked handler
        _httpClient = new HttpClient(_httpMessageHandlerMock)
        {
            BaseAddress = new Uri("http://mockapi.com"),
            Timeout = TimeSpan.FromSeconds(10)
        };
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        _service = new ForecastService(_httpClient, _configurationMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetTodaysSalesForecastAsync_Success_ReturnsSalesForecast()
    {
        // Arrange
        var expectedResponse = new SalesForecastResponse { PredictedSales = 1000m };
        var jsonResponse = JsonConvert.SerializeObject(expectedResponse);
        _httpMessageHandlerMock
            .When("/predict")
            .Respond(HttpStatusCode.OK, "application/json", jsonResponse);

        // Act
        var result = await _service.GetTodaysSalesForecastAsync();

        // Assert
        Xunit.Assert.NotNull(result);
        Xunit.Assert.Equal(expectedResponse.PredictedSales, result.daily_sales);
    }

    [Fact]
    public async Task GetTodaysSalesForecastAsync_HttpError_ReturnsDefaultResponse()
    {
        // Arrange
        _httpMessageHandlerMock
            .When("/predict")
            .Respond(HttpStatusCode.InternalServerError);

        // Act
        var result = await _service.GetTodaysSalesForecastAsync();

        // Assert
        Xunit.Assert.NotNull(result);
        Xunit.Assert.Equal(0m, result.daily_sales); // Assuming default is 0
        _loggerMock.VerifyLog(LogLevel.Error, "Error while fetching sales forecast", Times.Once());
    }

    [Fact]
    public async Task GetTodaysSalesForecastAsync_NullResponse_ReturnsDefaultResponse()
    {
        // Arrange
        _httpMessageHandlerMock
            .When("/predict")
            .Respond(HttpStatusCode.OK, "application/json", "null");

        // Act
        var result = await _service.GetTodaysSalesForecastAsync();

        // Assert
        Xunit.Assert.NotNull(result);
        Xunit.Assert.Equal(0m, result.daily_sales); // Assuming default is 0
    }

    [Fact]
    public async Task GetTodaysSalesForecastAsync_Timeout_ReturnsDefaultResponse()
    {
        // Arrange
        _httpMessageHandlerMock
            .When("/predict")
            .Throw(new TaskCanceledException("Request timed out"));

        // Act
        var result = await _service.GetTodaysSalesForecastAsync();

        // Assert
        Xunit.Assert.NotNull(result);
        Xunit.Assert.Equal(0m, result.daily_sales); // Assuming default is 0
        _loggerMock.VerifyLog(LogLevel.Error, "Error while fetching sales forecast", Times.Once());
    }

    [Fact]
    public async Task GetTodaysSalesForecastAsync_InvalidJson_ReturnsDefaultResponse()
    {
        // Arrange
        _httpMessageHandlerMock
            .When("/predict")
            .Respond(HttpStatusCode.OK, "application/json", "invalid json");

        // Act
        var result = await _service.GetTodaysSalesForecastAsync();

        // Assert
        Xunit.Assert.NotNull(result);
        Xunit.Assert.Equal(0m, result.daily_sales); // Assuming default is 0
        _loggerMock.VerifyLog(LogLevel.Error, "Error while fetching sales forecast", Times.Once());
    }
}

// Helper to verify logger calls (reused from previous examples)
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

// Assuming this is the response model (adjust as per your actual implementation)
public class SalesForecastResponse
{
    public decimal PredictedSales { get; set; } // Example property
}