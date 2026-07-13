using CommonLibrary.Exceptions;
using CommonRepo.Domain.Interfaces;
using docker_demo.Services.Interfaces;

namespace docker_demo.Services.Implementations
{
    public class WaybillReportService : IWaybillReportService
    {
        private readonly IEWaybillsViewRepository _eWaybillsViewRepository;
        private readonly IWaybillPdfService _waybillPdfService;

        public WaybillReportService(IEWaybillsViewRepository eWaybillsViewRepository, IWaybillPdfService waybillPdfService)
        {
            _eWaybillsViewRepository = eWaybillsViewRepository;
            _waybillPdfService = waybillPdfService;
        }

        public async Task<byte[]> GeneratePdfByIdAsync(int id)
        {
            var waybill = await _eWaybillsViewRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Waybill with id {id} was not found.");

            var request = waybill.ToWaybillPdfRequest();

            return await _waybillPdfService.GeneratePdfAsync(request);
        }
    }
}
