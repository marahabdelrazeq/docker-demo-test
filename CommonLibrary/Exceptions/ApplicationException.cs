using CommonLibrary.Enums;
using CommonLibrary.Responses;

namespace CommonLibrary.Exceptions
{
    /// <summary>
    /// Represents an application-level exception with custom status codes and error handling.
    /// </summary>
    public class ApplicationException : Exception
    {
        /// <summary>
        /// The status code of the exception.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// List of errors associated with the exception.
        /// </summary>
        public List<Error> Errors { get; }

        /// <summary>
        /// The original exception, if any, that caused this exception.
        /// </summary>
        public Exception Ex { get; } = null;

        /// <summary>
        /// Initializes a new instance of the ApplicationException class with a status code and a message.
        /// </summary>
        public ApplicationException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
            Errors = new List<Error>
            {
                new Error (statusCode, message)
            };
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationException class with a status code, a list of errors, and an optional inner exception.
        /// </summary>
        public ApplicationException(int statusCode, List<Error> errors, Exception ex = null)
        {
            StatusCode = statusCode;
            Errors = errors;
            Ex = ex;
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationException class with a Status enum and a message.
        /// </summary>
        public ApplicationException(Status statusCode, string message) : base(message)
        {
            StatusCode = (int)statusCode;
            Errors = new List<Error>
            {
                new Error (statusCode, message)
            };
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationException class with a Status enum, a list of errors, and an optional inner exception.
        /// </summary>
        public ApplicationException(Status statusCode, List<Error> errors, Exception ex = null)
        {
            StatusCode = (int)statusCode;
            Errors = errors;
            Ex = ex;
        }
    }
}
