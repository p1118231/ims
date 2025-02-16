using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;
namespace inventory.Services.SalesPrediction;

public class ForecastService: IForecastService
{
    private readonly HttpClient _client;

    public ForecastService(HttpClient client, IConfiguration configuration)
    {
        
        _client = client;
        var baseUrl = configuration["PredictionService:BaseUrl"]?? "";
        client.BaseAddress = new System.Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        
    }

    public async Task<SalesForecastResponse> GetTodaysSalesForecastAsync()
    {
        HttpResponseMessage response = await _client.GetAsync("/predict");
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var salesForecast = JsonConvert.DeserializeObject<SalesForecastResponse>(jsonResponse);
        return salesForecast ?? new SalesForecastResponse();
    }
}
