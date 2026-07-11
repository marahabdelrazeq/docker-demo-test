using CommonLibrary.Enums;
using CommonLibrary.Exceptions;
using CommonLibrary.RequestInformation;
using CommonLibrary.Responses;
using CommonLibrary.Utilities.StringHelper;
using CommonLibrary.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Error = CommonLibrary.Responses.Error;
using StringConverter = CommonLibrary.Utilities.StringHelper.StringConverter;
using ValidationException = CommonLibrary.Exceptions.ValidationException;

namespace CommonLibrary.Controllers
{
    /// <summary>
    /// Base controller that provides common functionality such as request information handling and response wrapping.
    /// </summary>
    public class BaseController : Controller
    {
        private readonly IRequestInfoService _requestInfoService;
        private readonly IActionContextAccessor _actionContextAccessor;

        public BaseController(IRequestInfoService requestInfoService, IActionContextAccessor actionContextAccessor)
        {
            _requestInfoService = requestInfoService;
            _actionContextAccessor = actionContextAccessor;
        }

        /// <summary>
        /// Overrides the OnActionExecuting method to set request information and handle model validation.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            SetRequestInfo(context);
            base.OnActionExecuting(context);

            if (!context.ModelState.IsValid)
            {
                var errors = new List<Error>();

                // Build a lookup of LocationMessage attributes from the action parameter types
                var locationMessageLookup = BuildLocationMessageLookup(context);

                foreach (var entry in context.ModelState)
                {
                    if (entry.Value.Errors.Any())
                    {
                        var path = entry.Key;

                        // Try to resolve a custom locationMessage from the attribute, fallback to path
                        var locationMessage = ResolveLocationMessage(locationMessageLookup, path);

                        foreach (var error in entry.Value.Errors)
                        {
                            var errorMessage = error.ErrorMessage;
                            if (errorMessage.Contains("Error converting value", StringComparison.OrdinalIgnoreCase))
                            {
                                errors.Add(new Error(Status.BadRequest, locationMessage, $"{path}: Invalid JSON format or type mismatch"));
                            }
                            else
                            {
                                errors.Add(new Error(Status.BadRequest, locationMessage, errorMessage));
                            }
                        }
                    }
                }

                throw new ValidationException(errors);
            }
        }

        /// <summary>
        /// Builds a dictionary mapping property names to their [LocationMessage] values
        /// from the action method's parameter types.
        /// </summary>
        [NonAction]
        private static Dictionary<string, string> BuildLocationMessageLookup(ActionExecutingContext context)
        {
            var lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var parameter in context.ActionDescriptor.Parameters)
            {
                var paramType = parameter.ParameterType;

                foreach (var prop in paramType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var attr = prop.GetCustomAttribute<LocationMessageAttribute>();
                    if (attr != null)
                    {
                        // Key could be just "PropertyName" or "ParameterName.PropertyName"
                        lookup[prop.Name] = attr.Message;
                    }
                }
            }

            return lookup;
        }

        /// <summary>
        /// Resolves the location message for a given ModelState key.
        /// Falls back to the key (path) itself if no [LocationMessage] attribute is found.
        /// </summary>
        [NonAction]
        private static string ResolveLocationMessage(Dictionary<string, string> lookup, string path)
        {
            // ModelState key can be "PropertyName" or "dto.PropertyName"
            // Try exact match first, then try the last segment
            if (lookup.TryGetValue(path, out var message))
                return message;

            var lastDot = path.LastIndexOf('.');
            if (lastDot >= 0)
            {
                var propertyName = path.Substring(lastDot + 1);
                if (lookup.TryGetValue(propertyName, out message))
                    return message;
            }

            return path;
        }

        /// <summary>
        /// Sets request information using the HTTP context headers and updates the IRequestInfoService.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        [NonAction]
        private void SetRequestInfo(ActionExecutingContext context)
        {
            var requestHeaders = context.HttpContext.Request.Headers;
            var requestInfo = _requestInfoService.GetRequestInfo();

            requestInfo.ServerIpAddress = context.HttpContext.Connection.LocalIpAddress?.MapToIPv4().ToString() ?? "Unknown";
            requestInfo.ClientIpAddress = context.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";
            requestInfo.UserAgent = requestHeaders["User-Agent"].ToString();

            if (requestHeaders.TryGetValue("CountryCode", out var countryCode))
            {
                requestInfo.CountryCode = countryCode!;
            }

            if (requestHeaders.TryGetValue("Ownership", out var ownership))
            {
                requestInfo.Ownership = ownership!;

                requestInfo.Ownership = requestInfo.Ownership.Replace($"#CountryCode#", requestInfo.CountryCode);
            }
            if (requestHeaders.TryGetValue("ApplicationName", out var applicationName))
            {
                requestInfo.ApplicationName = applicationName!;
            }

            if (requestHeaders.TryGetValue("LoginProvider", out var source))
            {
                requestInfo.Source = source!;
            }

            if (requestHeaders.TryGetValue("Name", out var deviceId))
            {
                requestInfo.DeviceId = deviceId!;
            }

            if (requestHeaders.TryGetValue("ClientInfo", out var clientInfo))
            {
                var combinedHeaders = clientInfo.ToString();

                if (!string.IsNullOrEmpty(combinedHeaders))
                {
                    string[] values = combinedHeaders.Split("|");

                    requestInfo.ApplicationName = values[0];

                    requestInfo.Source = values[1];

                    requestInfo.CountryCode = values[2];

                    requestInfo.ServerIpAddress = values[3];

                    requestInfo.ClientIpAddress = values[4];

                }
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
                    requestInfo.ApplicationName = values[5];
                    requestInfo.Source = values[6];
                }
            }

            requestInfo.ActionName = StringConverter.ConvertToUpperCaseSnakeCase(
                _actionContextAccessor.ActionContext?.RouteData?.Values["action"]?.ToString());

            _requestInfoService.SetRequestInfo(requestInfo);
        }

        /// <summary>
        /// Returns a formatted OK response with a message.
        /// </summary>
        /// <param name="message">Optional message for the response.</param>
        /// <returns>A formatted API response.</returns>
        protected IActionResult _Ok(string message = null) => base.Ok(new ApiResponse<string>(message, null));

        /// <summary>
        /// Returns a formatted OK response with paginated data.
        /// </summary>
        /// <typeparam name="TData">The type of the data being returned.</typeparam>
        /// <param name="data">A tuple containing data and total count.</param>
        /// <returns>A formatted API response.</returns>
        protected IActionResult _Ok<TData>((IEnumerable<TData>, int) data) => base.Ok(new ApiResponse<TData>(data, null));

        /// <summary>
        /// Returns a formatted OK response with data.
        /// </summary>
        /// <typeparam name="TData">The type of the data being returned.</typeparam>
        /// <param name="data">The data to return in the response.</param>
        /// <returns>A formatted API response.</returns>
        protected IActionResult _Ok<TData>(TData data) => base.Ok(new ApiResponse<TData>(data, null));
    }
}
