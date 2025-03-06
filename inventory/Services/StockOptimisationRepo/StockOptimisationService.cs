using System.Text.Json;

namespace inventory.Services.StockOptimisationRepo
{
   public class StockOptimisationService : IStockOptimisationService
   {
     private readonly HttpClient _client;

    public StockOptimisationService(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        var baseUrl = configuration["StockService:BaseUrl"]?? "";
        client.BaseAddress = new System.Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<StockOptimisationResponse> PredictStockLevelAsync(int productId)
    {
        try {
        HttpResponseMessage response = await _client.GetAsync($"predict/{productId}");
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var prediction = JsonSerializer.Deserialize<StockOptimisationResponse>(responseBody);
        Console.WriteLine(prediction.predicted_stock_level);
        return prediction?? new StockOptimisationResponse();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
            return new StockOptimisationResponse();
        }
   }

        
      
    }}