using CommonLibrary.Controllers;
using CommonLibrary.RequestInformation;
using docker_demo.DTOs.WaybillDto;
using docker_demo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace docker_demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaybillController : BaseController
    {
        private readonly IWaybillPdfService _waybillPdfService;

        public WaybillController(IRequestInfoService requestInfoService, IActionContextAccessor actionContextAccessor, IWaybillPdfService waybillPdfService)
            : base(requestInfoService, actionContextAccessor)
        {
            _waybillPdfService = waybillPdfService;
        }

        [HttpPost("Pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GeneratePdf([FromBody] WaybillPdfRequest request)
        {
            var pdfBytes = await _waybillPdfService.GeneratePdfAsync(request);

            return File(pdfBytes, "application/pdf", "waybill.pdf");
        }
    }
}
