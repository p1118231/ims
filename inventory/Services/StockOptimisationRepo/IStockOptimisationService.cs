namespace inventory.Services.StockOptimisationRepo
{
    public interface IStockOptimisationService
    {
        Task<StockOptimisationResponse> PredictStockLevelAsync(int productId);
    }
}