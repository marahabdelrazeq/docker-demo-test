using docker_demo.DTOs.WaybillDto;

namespace docker_demo.Services.Interfaces
{
    public interface IWaybillPdfService
    {
        Task<byte[]> GeneratePdfAsync(WaybillPdfRequest request);
    }
}
