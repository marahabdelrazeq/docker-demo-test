using CommonLibrary.Enums;
using CommonLibrary.RequestInformation;
using CommonLibrary.Responses;
using CommonLibrary.Utilities.StringHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CommonRepo.Application.FilterAttribute
{
    public class RequestInfoFilterAttribute(IRequestInfoService requestInfoService, IActionContextAccessor actionContextAccessor) : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            SetRequestInfo(context);
            base.OnActionExecuting(context);
            if (!context.ModelState.IsValid)
            {

                List<Error> errors = new List<Error>();

                foreach (var entry in context.ModelState)
                {
                    if (entry.Value.Errors.Any())
                    {
                        foreach (var error in entry.Value.Errors)
                        {
                            errors.Add(new Error
                            (
                                Status.BadRequest,
                                 error.ErrorMessage
                            ));
                        }
                    }
                }

                context.Result = new BadRequestObjectResult(new ApiResponse<object>(null, errors));
            }


        }

        private void SetRequestInfo(ActionExecutingContext context)
        {
            var requestHeaders = context.HttpContext.Request.Headers;
            var requestInfo = requestInfoService.GetRequestInfo();

            requestInfo.ServerIpAddress = context.HttpContext.Connection.LocalIpAddress?.MapToIPv4().ToString() ?? "Unknown";
            requestInfo.ClientIpAddress = context.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";
            requestInfo.UserAgent = requestHeaders["User-Agent"].ToString();

            if (requestHeaders.TryGetValue("CountryCode", out var countryCode))
            {
                requestInfo.CountryCode = countryCode!;
            }

            if (requestHeaders.TryGetValue("LoginProvider", out var source))
            {
                requestInfo.Source = source!;
            }

            if (requestHeaders.TryGetValue("Ownership", out var ownership))
            {
                requestInfo.Ownership = ownership!;

                requestInfo.Ownership = requestInfo.Ownership.Replace($"#CountryCode#", requestInfo.CountryCode);
            }

            if (requestHeaders.TryGetValue("CombinedHeaders", out var headerValues))
            {
                var combinedHeaders = headerValues.ToString();

                if (!string.IsNullOrEmpty(combinedHeaders))
                {
                    var values = combinedHeaders.Split("|");

                    requestInfo.UserId = int.Parse(values[0]);
                    requestInfo.UserRole = values[1];
                    requestInfo.StakeholderId = int.Parse(values[2]);
                    requestInfo.TableName = values[3];
                    requestInfo.IsRegistered = bool.Parse(values[4]);
                }
            }

            requestInfo.ActionName = StringConverter.ConvertToUpperCaseSnakeCase(
                actionContextAccessor.ActionContext?.RouteData?.Values["action"]?.ToString());

            requestInfoService.SetRequestInfo(requestInfo);
        }
    }
}
