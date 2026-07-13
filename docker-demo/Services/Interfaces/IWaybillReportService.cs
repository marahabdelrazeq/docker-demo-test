namespace docker_demo.Services.Interfaces
{
    public interface IWaybillReportService
    {
        Task<byte[]> GeneratePdfByIdAsync(int id);
    }
}
