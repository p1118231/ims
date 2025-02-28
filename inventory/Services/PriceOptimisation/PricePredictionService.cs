using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using inventory.Services.PriceOptimisation;

namespace inventory.Services.PriceOptimisation;

public class PricePredictionService: IPricePredictionService
{
    private readonly HttpClient _client;

    public PricePredictionService(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        var baseUrl = configuration["PriceService:BaseUrl"]?? "";
        client.BaseAddress = new System.Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<PricePredictionResponse> PredictPriceAsync(int productId)
    {
        try {
        HttpResponseMessage response = await _client.GetAsync($"predict/{productId}");
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var prediction = JsonSerializer.Deserialize<PricePredictionResponse>(responseBody);
        //Console.WriteLine(prediction.PredictedPrice);
        return prediction?? new PricePredictionResponse();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
            return new PricePredictionResponse();
        }
    }

    
}