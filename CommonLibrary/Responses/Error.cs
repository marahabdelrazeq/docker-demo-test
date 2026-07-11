using CommonLibrary.Enums;

namespace CommonLibrary.Responses
{
    /// <summary>
    /// Class for capturing errors in the API response.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Status code for the error.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage { get; set; }


        ///<summary>
        ///Location message for the response.
        ///<summary>
        public string LocationMessage { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Error() { }

        /// <summary>
        /// Constructor with a status code enum and error message.
        /// </summary>
        public Error(Status statusCode, string errorMessage)
        {
            StatusCode = (int)statusCode;
            ErrorMessage = errorMessage;
        }

        public Error(Status statusCode, string locationMessage, string errorMessage)
        {
            StatusCode = (int)statusCode;
            ErrorMessage = errorMessage;
            LocationMessage = locationMessage;
        }

        /// <summary>
        /// Constructor with a status code integer and error message.
        /// </summary>
        public Error(int statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
