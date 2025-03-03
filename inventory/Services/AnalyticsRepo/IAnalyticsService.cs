namespace inventory.Services.AnalyticsRepo;

public interface IAnalyticsService
{
    Task<AnalyticsDto> GetAnalytics();
}