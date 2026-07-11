using CommonLibrary.Enums;
using CommonLibrary.Responses;

namespace CommonLibrary.Exceptions
{
    /// <summary>
    /// Represents a system-level exception, initialized with a list of errors and an optional inner exception.
    /// </summary>
    public class SystemException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the SystemException class with a list of errors and an optional inner exception.
        /// </summary>
        /// <param name="errors">A list of errors associated with the exception.</param>
        /// <param name="ex">The inner exception that caused this exception.</param>
        public SystemException(List<Error> errors, Exception ex)
            : base(Status.BadRequest, errors, ex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SystemException class with a custom status, list of errors, and an optional inner exception.
        /// </summary>
        /// <param name="status">The status code of the system exception.</param>
        /// <param name="errors">A list of errors associated with the exception.</param>
        /// <param name="ex">The inner exception that caused this exception.</param>
        public SystemException(Status status, List<Error> errors, Exception ex = null)
            : base(status, errors, ex)
        {
        }
    }
}
