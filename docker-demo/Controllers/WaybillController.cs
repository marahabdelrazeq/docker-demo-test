using CommonLibrary.Controllers;
using CommonLibrary.RequestInformation;
using docker_demo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace docker_demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaybillController : BaseController
    {
        private readonly IWaybillReportService _waybillReportService;

        public WaybillController(IRequestInfoService requestInfoService, IActionContextAccessor actionContextAccessor, IWaybillReportService waybillReportService)
            : base(requestInfoService, actionContextAccessor)
        {
            _waybillReportService = waybillReportService;
        }

        [HttpGet("Pdf/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PrintWaybill(int id)
        {
            var filePath = await _waybillReportService.GeneratePdfByIdAsync(id);

            return _Ok(filePath);
        }
    }
}
