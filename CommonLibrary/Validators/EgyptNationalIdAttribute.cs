using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Custom validation attribute that ensures a National ID follows Egyptian format.
    /// Egyptian National ID: exactly 14 digits
    /// </summary>
    public class EgyptNationalIdAttribute : ValidationAttribute
    {
        /// <summary>
        /// Performs the validation to check if the National ID follows Egyptian format.
        /// </summary>
        /// <param name="value">The value being validated (must be a string).</param>
        /// <param name="validationContext">Contextual information about the validation operation.</param>
        /// <returns>A ValidationResult indicating success or failure.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // If the value is null or empty, consider it valid (use [Required] for mandatory fields)
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success!;
            }

            string nationalId = value.ToString()!.Trim();

            // Remove any non-digit characters for validation
            string digitsOnly = Regex.Replace(nationalId, @"[^\d]", "");

            // Egyptian National ID must be exactly 14 digits
            if (!Regex.IsMatch(digitsOnly, @"^\d{14}$"))
            {
                return new ValidationResult("INVALID_EGYPT_NATIONAL_ID_FORMAT");
            }

            return ValidationResult.Success!;
        }
    }
}
