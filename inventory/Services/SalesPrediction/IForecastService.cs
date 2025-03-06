namespace inventory.Services.SalesPrediction
{
    public interface IForecastService
    {
        Task<SalesForecastResponse> GetTodaysSalesForecastAsync();
    }
} 