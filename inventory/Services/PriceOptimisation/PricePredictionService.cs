using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using inventory.Services.PriceOptimisation;

namespace inventory.Services.PriceOptimisation;

public class PricePredictionService: IPricePredictionService
{
    private readonly HttpClient _client;

    // Constructor to initialize HttpClient and set its properties
    public PricePredictionService(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        var baseUrl = configuration["PriceService:BaseUrl"] ?? "";
        client.BaseAddress = new System.Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    // Method to predict price asynchronously for a given productId
    public async Task<PricePredictionResponse> PredictPriceAsync(int productId)
    {
        try
        {
            // Send GET request to the prediction service
            HttpResponseMessage response = await _client.GetAsync($"predict/{productId}");
            response.EnsureSuccessStatusCode();

            // Read and deserialize the response body
            var responseBody = await response.Content.ReadAsStringAsync();
            var prediction = JsonSerializer.Deserialize<PricePredictionResponse>(responseBody);

            // Return the prediction or a new PricePredictionResponse if null
            return prediction ?? new PricePredictionResponse();
        }
        catch (HttpRequestException e)
        {
            // Log the error and return a new PricePredictionResponse
            Console.WriteLine($"An error occurred: {e.Message}");
            return new PricePredictionResponse();
        }
        catch (Exception e)
        {
            // Catch any other exceptions, log the error and return a new PricePredictionResponse
            Console.WriteLine($"An unexpected error occurred: {e.Message}");
            return new PricePredictionResponse();
        }
    }
}