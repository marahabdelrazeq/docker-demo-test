using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Custom validation attribute that ensures the input contains only numeric characters, with an optional digit limit.
    /// </summary>
    public class NumberOnly : ValidationAttribute
    {
        /// <summary>
        /// Maximum number of digits allowed in the validated string (default is 25).
        /// </summary>
        public int NumberOfDigits { get; set; } = 25;

        /// <summary>
        /// Initializes a new instance of the NumberOnly validation attribute with the default digit limit.
        /// </summary>
        public NumberOnly() { }

        /// <summary>
        /// Initializes a new instance of the NumberOnly validation attribute with a custom digit limit.
        /// </summary>
        /// <param name="numberOfDigits">The maximum number of digits allowed in the validated string.</param>
        public NumberOnly(int numberOfDigits)
        {
            NumberOfDigits = numberOfDigits;
        }

        /// <summary>
        /// Performs the validation to check if the value contains only numeric characters, within the specified digit limit.
        /// </summary>
        /// <param name="value">The value being validated.</param>
        /// <param name="validationContext">Contextual information about the validation operation.</param>
        /// <returns>A ValidationResult indicating success or failure.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                // Regex to check if the string contains only numeric characters, within the digit limit
                Regex characterRegex = new($@"^[0-9]{{1,{NumberOfDigits}}}$");
                bool isMatch = characterRegex.IsMatch(value.ToString()!);

                if (!isMatch)
                {
                    // Return a validation failure with a custom error message
                    return new ValidationResult($"THE_FIELD_{validationContext.MemberName!.ToUpper()}_MUST_CONTAIN_UP_TO_{NumberOfDigits}_NUMERIC_CHARACTERS");
                }

                // Return success if the validation passes
                return ValidationResult.Success;
            }

            return null; // Return null if the value is null, meaning no validation is needed
        }
    }
}
