using System.Text.Json.Serialization;

namespace inventory.Services.AnalyticsRepo;

public class SalesTrendDto{
    [JsonPropertyName("date")]
    public string? Date {get; set;}
    [JsonPropertyName("totalSales")]
    public decimal TotalSales {get; set;}

    public IEnumerable<SalesTrendDto>? SalesTrend {get; set;}
}