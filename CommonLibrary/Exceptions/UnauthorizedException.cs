using CommonLibrary.Enums;
using CommonLibrary.Responses;

namespace CommonLibrary.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when a requested resource is not found.
    /// </summary>
    public class UnauthorizedException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the NotFoundException class with a custom message.
        /// </summary>
        /// <param name="message">The error message describing the resource that was not found.</param>
        public UnauthorizedException(string message)
            : base(Status.Unauthorized, new List<Error> { new Error(Status.Unauthorized, message) })
        {
        }
    }
}
