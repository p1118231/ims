namespace inventory.Services.ReportRepo
{
    public interface IReportService
    {
        Task<ReportDto> GetReport();
    }
}