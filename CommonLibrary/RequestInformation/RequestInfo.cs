namespace CommonLibrary.RequestInformation
{
    /// <summary>
    /// Represents the information related to a request, including user, IP, and action details.
    /// </summary>
    public class RequestInfo
    {
        public int UserId { get; set; }
        public string UserRole { get; set; }
        public string ServerIpAddress { get; set; }
        public string Ownership { get; set; }
        public string TableName { get; set; }
        public int StakeholderId { get; set; }
        public bool IsRegistered { get; set; }
        public string LanguageCode { get; set; }
        public string ApplicationName { get; set; }
        public string ClientIpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Source { get; set; }
        public string CountryCode { get; set; }
        public string ActionName { get; set; }
        public bool EnableLogging { get; set; }
        public string RequestUrl { get; set; }
        public string DeviceId { get; set; }

        /// <summary>
        /// Initializes a new instance of the RequestInfo class with default values.
        /// </summary>
        public RequestInfo()
        {
            UserId = 0;
            UserRole = string.Empty;
            StakeholderId = 0;
            Ownership = "false";
            TableName = string.Empty;
            ApplicationName = string.Empty;
            Source = string.Empty;
            IsRegistered = false;
            CountryCode = string.Empty;
            UserAgent = string.Empty;
            ServerIpAddress = "127.0.0.1";
            ClientIpAddress = "127.0.0.1";
            ActionName = string.Empty;
            LanguageCode = "En";
            RequestUrl = string.Empty;
            EnableLogging = false;
            DeviceId = string.Empty;
        }
    }
}
