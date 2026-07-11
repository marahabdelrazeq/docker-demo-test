using CommonLibrary.Enums;
using CommonLibrary.Responses;

namespace CommonLibrary.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when validation fails.
    /// </summary>
    public class ValidationException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the ValidationException class with a list of validation errors.
        /// </summary>
        /// <param name="errors">A list of validation errors.</param>
        public ValidationException(List<Error> errors)
            : base(Status.UnprocessableEntity, errors)
        {
        }
        public ValidationException(Error error)
             : base(Status.UnprocessableEntity, error.ErrorMessage)
        {
        }
        public ValidationException(string errorMessage)
            : base(Status.UnprocessableEntity, errorMessage)
        {
        }
    }
}
