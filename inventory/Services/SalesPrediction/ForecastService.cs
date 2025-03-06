using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;
namespace inventory.Services.SalesPrediction;

public class ForecastService: IForecastService
{
    private readonly HttpClient _client;
    private readonly ILogger<ForecastService> _logger;

    public ForecastService(HttpClient client, IConfiguration configuration, ILogger<ForecastService> logger)
    {
        
        _client = client;
        var baseUrl = configuration["PredictionService:BaseUrl"]?? "";
        client.BaseAddress = new System.Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        _logger = logger;
        
    }

    public async Task<SalesForecastResponse> GetTodaysSalesForecastAsync()
    {
        try{
        HttpResponseMessage response = await _client.GetAsync("/predict");
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var salesForecast = JsonConvert.DeserializeObject<SalesForecastResponse>(jsonResponse);
        return salesForecast ?? new SalesForecastResponse();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            _logger.LogError(ex, "Error while fetching sales forecast");
            return new SalesForecastResponse();
        }
    }
}
