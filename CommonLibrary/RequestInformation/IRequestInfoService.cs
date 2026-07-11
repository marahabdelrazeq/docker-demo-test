namespace CommonLibrary.RequestInformation
{
    /// <summary>
    /// Defines methods for retrieving and updating request information.
    /// </summary>
    public interface IRequestInfoService
    {
        /// <summary>
        /// Retrieves the current request information.
        /// </summary>
        /// <returns>An instance of RequestInfo representing the current request.</returns>
        RequestInfo GetRequestInfo();

        /// <summary>
        /// Sets the current request information.
        /// </summary>
        /// <param name="requestInfo">The RequestInfo instance to be set.</param>
        void SetRequestInfo(RequestInfo requestInfo);
    }
}
