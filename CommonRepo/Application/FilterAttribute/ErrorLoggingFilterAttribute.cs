using CommonLibrary.Enums;
using CommonLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CommonRepo.Domain.Entities; 
using CommonRepo.Domain.Interfaces;

namespace CommonRepo.Application.FilterAttribute
{
    public class ErrorLoggingFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IErrorLoggingService _errorLoggingService;

        // Assuming dependency injection is correctly set up to inject this service
        public ErrorLoggingFilterAttribute(IErrorLoggingService errorLoggingService)
        {
            _errorLoggingService = errorLoggingService;
        }

        public override void OnException(ExceptionContext context)
        {

            if (context.ExceptionHandled) return;

            var controllerName = context.RouteData.Values["controller"].ToString();
            var actionName = context.RouteData.Values["action"].ToString();

            // Log the exception details to the database
            _errorLoggingService.CreateSystemErrorLogAsync(new SystemErrorsLogDTO() { ControllerName = controllerName, RequestAPI = actionName, Args = controllerName, InnerEx = context.Exception });

            // Set the context result to BadRequest (400) with the custom error response
            context.Result = new BadRequestObjectResult(new ApiResponse<object>(null, new List<Error> { new(Status.InternalServerError, context.Exception.InnerException?.Message ?? context.Exception.Message) }));

            // Mark the exception as handled to stop further propagation
            context.ExceptionHandled = true;
        }
    }
}
