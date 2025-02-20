

namespace inventory.Services.PriceOptimisation

{
    public interface IPricePredictionService
    {
        Task<PricePredictionResponse> PredictPriceAsync(int productId);
    }
}