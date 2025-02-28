

namespace inventory.Services.DashboardRepo
{
    public interface IDashboardService
    {
        Task<HomeViewModel> GetDashboardData();
    }
}