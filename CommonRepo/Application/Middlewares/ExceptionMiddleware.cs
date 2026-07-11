using CommonLibrary.Enums;
using CommonLibrary.Responses;
using CommonRepo.Domain.Entities;
using CommonRepo.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using ApplicationException = CommonLibrary.Exceptions.ApplicationException;
using SystemException = CommonLibrary.Exceptions.SystemException;

namespace CommonRepo.Application.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IErrorLoggingService _errorLoggingService;

        public ExceptionMiddleware(RequestDelegate next, IErrorLoggingService errorLoggingService)
        {
            _next = next;
            _errorLoggingService = errorLoggingService;
        }

        public async Task Invoke(HttpContext context)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            var routeData = context.GetRouteData();

            string controllerName = routeData?.Values["controller"]?.ToString();

            string actionName = routeData?.Values["action"]?.ToString();

            try
            {
                await _next(context);
            }
            catch (ApplicationException ex)
            {
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = (int)ex.StatusCode;

                var response = new ApiResponse<object>(null, ex.Errors);

                if (ex is SystemException sysExp)
                {
                    _ = _errorLoggingService.CreateSystemErrorLogAsync(new SystemErrorsLogDTO() { ControllerName = controllerName, RequestAPI = actionName, Args = controllerName, InnerEx = sysExp.Ex });
                }
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response, settings));
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = new ApiResponse<object>(null, new List<Error>
            {
                new Error(Status.InternalServerError, ex.InnerException!=null?ex.InnerException.Message:ex.Message)
            });
                _ = _errorLoggingService.CreateSystemErrorLogAsync(new SystemErrorsLogDTO() { ControllerName = controllerName, RequestAPI = actionName, Args = controllerName, InnerEx = ex });
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response, settings));
            }
        }
    }

}
