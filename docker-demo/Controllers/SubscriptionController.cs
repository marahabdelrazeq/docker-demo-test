
using CommonLibrary.Controllers;
using CommonLibrary.RequestInformation;
using docker_demo.DTOs.SubscriptionDto;
using docker_demo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
namespace docker_demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : BaseController
    {

        ISubscriptionService _subService;
        public SubscriptionController(IRequestInfoService requestInfoService, IActionContextAccessor actionContextAccessor, ISubscriptionService subService) : base(requestInfoService, actionContextAccessor)
        {
            _subService = subService;
        }

        [HttpPost("ValidateTruck")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateTruck([FromBody] SubscriptionDTO subDto)
        {
            var output = await _subService.ValidateTruck(subDto);

            return Ok(output);

        }

    }
}
