using CommonLibrary.Exceptions;
using CommonRepo.Domain.Interfaces;
using docker_demo.Services.Interfaces;

namespace docker_demo.Services.Implementations
{
    public class WaybillReportService : IWaybillReportService
    {
        private const string EntityFolderName = "waybill";

        private readonly IEWaybillsViewRepository _eWaybillsViewRepository;
        private readonly IWaybillPdfService _waybillPdfService;
        private readonly IConfiguration _configuration;

        public WaybillReportService(IEWaybillsViewRepository eWaybillsViewRepository, IWaybillPdfService waybillPdfService, IConfiguration configuration)
        {
            _eWaybillsViewRepository = eWaybillsViewRepository;
            _waybillPdfService = waybillPdfService;
            _configuration = configuration;
        }

        public async Task<string> GeneratePdfByIdAsync(int id)
        {
            var waybill = await _eWaybillsViewRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Waybill with id {id} was not found.");

            var request = waybill.ToWaybillPdfRequest();

            var pdfBytes = await _waybillPdfService.GeneratePdfAsync(request);

            var folderPath = Path.Combine(_configuration["AttachmentSettings:FolderPath"]!, EntityFolderName);
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, $"{Guid.NewGuid()}.pdf");
            await File.WriteAllBytesAsync(filePath, pdfBytes);

            return filePath;
        }
    }
}
