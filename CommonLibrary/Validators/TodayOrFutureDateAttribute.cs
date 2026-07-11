using System;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Custom validation attribute that ensures a date is today or in the future.
    /// </summary>
    public class TodayOrFutureDateAttribute : ValidationAttribute
    {
        /// <summary>
        /// Performs the validation to check if the date is today or in the future.
        /// </summary>
        /// <param name="value">The value being validated (must be a DateTime or nullable DateTime).</param>
        /// <param name="validationContext">Contextual information about the validation operation.</param>
        /// <returns>A ValidationResult indicating success or failure.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // If the value is null, consider it valid (use [Required] for mandatory fields)
            if (value == null)
            {
                return ValidationResult.Success!;
            }

            DateTime dateValue;

            // Try to parse the value as DateTime
            if (value is DateTime dt)
            {
                dateValue = dt;
            }
            else if (DateTime.TryParse(value.ToString(), out DateTime parsedDate))
            {
                dateValue = parsedDate;
            }
            else
            {
                return new ValidationResult("INVALID_DATE_FORMAT");
            }

            // Check if the date is today or in the future
            if (dateValue.Date < DateTime.Today)
            {
                return new ValidationResult("DATE_MUST_BE_TODAY_OR_FUTURE");
            }

            return ValidationResult.Success!;
        }
    }
}
