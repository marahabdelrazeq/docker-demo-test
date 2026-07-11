using Microsoft.AspNetCore.Http;

namespace CommonLibrary.RequestInformation
{
    /// <summary>
    /// Provides methods to get and set request information in the HttpContext.
    /// </summary>
    public class RequestInfoService : IRequestInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the RequestInfoService class.
        /// </summary>
        /// <param name="httpContextAccessor">The IHttpContextAccessor to access the current HttpContext.</param>
        public RequestInfoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieves the current request information from the HttpContext.
        /// </summary>
        /// <returns>An instance of RequestInfo containing the current request information.</returns>
        public RequestInfo GetRequestInfo()
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                var requestInfo = httpContext.Items["RequestInfo"] as RequestInfo;
                return requestInfo ?? new RequestInfo();
            }
            else
            {
                // HttpContext is null, return default request info with Ownership set to "true"
                RequestInfo requestInfo = new RequestInfo
                {
                    Ownership = "true"
                };
                return requestInfo;
            }
        }

        /// <summary>
        /// Sets the current request information in the HttpContext.
        /// </summary>
        /// <param name="requestInfo">The RequestInfo object to set in the HttpContext.</param>
        public void SetRequestInfo(RequestInfo requestInfo)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Items["RequestInfo"] = requestInfo;
            }
            else
            {
                // Handle case where HttpContext is null, log an error or take some action
                // Log warning or error here if needed
            }
        }
    }
}
