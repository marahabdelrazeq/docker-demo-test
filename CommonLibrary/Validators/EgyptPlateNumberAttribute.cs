using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Custom validation attribute for Egyptian vehicle plate numbers.
    /// Validates that the plate number contains 3-4 digits only.
    /// </summary>
    public class EgyptPlateNumberAttribute : ValidationAttribute
    {
        /// <summary>
        /// Performs the validation to check if the value is a valid Egyptian plate number.
        /// </summary>
        /// <param name="value">The value being validated.</param>
        /// <param name="validationContext">Contextual information about the validation operation.</param>
        /// <returns>A ValidationResult indicating success or failure.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                // If value is null or empty, let the Required attribute handle it
                return ValidationResult.Success;
            }

            string plateNumber = value.ToString()!.Trim();

            // Regex pattern: 3-4 digits only
            Regex plateNumberRegex = new Regex(@"^\d{3,4}$");

            if (!plateNumberRegex.IsMatch(plateNumber))
            {
                return new ValidationResult("PLATE_NUMBER_MUST_BE_3_TO_4_DIGITS");
            }

            return ValidationResult.Success;
        }
    }
}
