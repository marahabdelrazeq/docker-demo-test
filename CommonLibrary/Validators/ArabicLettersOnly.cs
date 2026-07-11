using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Custom validation attribute that ensures the input contains only Arabic letters and spaces,
    /// with an optional character limit.
    /// </summary>
    public class ArabicLettersOnly : ValidationAttribute
    {
        /// <summary>
        /// Maximum number of characters allowed in the validated string (default is 25).
        /// </summary>
        private int NumberOfCharacters { get; set; } = 25;

        /// <summary>
        /// Initializes a new instance of the ArabicLettersOnly validation attribute with the default character limit.
        /// </summary>
        public ArabicLettersOnly() { }

        /// <summary>
        /// Initializes a new instance of the ArabicLettersOnly validation attribute with a custom character limit.
        /// </summary>
        /// <param name="numberOfCharacters">The maximum number of characters allowed in the validated string.</param>
        public ArabicLettersOnly(int numberOfCharacters)
        {
            NumberOfCharacters = numberOfCharacters;
        }

        /// <summary>
        /// Performs the validation to check if the value contains only Arabic letters and spaces, within the specified character limit.
        /// </summary>
        /// <param name="value">The value being validated.</param>
        /// <param name="validationContext">Contextual information about the validation operation.</param>
        /// <returns>A ValidationResult indicating success or failure.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                // Regex to check if the string contains only Arabic letters and spaces, within the character limit
                Regex characterRegex = new($@"^[ءأ-ي\s]{{1,{NumberOfCharacters}}}$");
                bool isMatch = characterRegex.IsMatch(value.ToString()!);

                if (!isMatch)
                {
                    // Return a validation failure with a custom error message
                    return new ValidationResult($"THE_FIELD_{validationContext.MemberName!.ToUpper()}_MUST_CONTAIN_UP_TO_{NumberOfCharacters}_ARABIC_LETTERS");
                }

                // Return success if the validation passes
                return ValidationResult.Success;
            }

            return null; // Return null if the value is null, meaning no validation is needed
        }
    }
}
