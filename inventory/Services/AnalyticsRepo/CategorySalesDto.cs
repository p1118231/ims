using System.Text.Json.Serialization;

namespace inventory.Services.AnalyticsResp;

public class CategorySalesDto
{
    [JsonPropertyName("category")]
    public string? Category { get; set; }
    [JsonPropertyName("sales")]
    public decimal Sales { get; set; }
}