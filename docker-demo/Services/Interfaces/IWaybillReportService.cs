namespace docker_demo.Services.Interfaces
{
    public interface IWaybillReportService
    {
        Task<string> GeneratePdfByIdAsync(int id);
    }
}
