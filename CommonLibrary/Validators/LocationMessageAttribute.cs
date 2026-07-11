using System;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Specifies a custom location message for a property.
    /// This value is used to populate the LocationMessage field in validation error responses.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LocationMessageAttribute : Attribute
    {
        /// <summary>
        /// The location message value.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the LocationMessageAttribute with the specified message.
        /// </summary>
        /// <param name="message">The location message to include in validation error responses.</param>
        public LocationMessageAttribute(string message)
        {
            Message = message;
        }
    }
}
