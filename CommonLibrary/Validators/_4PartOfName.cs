using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Custom validation attribute that checks if a string contains exactly four parts (each part is a word).
    /// </summary>
    public partial class _4PartOfName : ValidationAttribute
    {
        /// <summary>
        /// Maximum number of characters allowed in the validated string.
        /// </summary>
        public int NumberOfCharacters { get; set; } = 25;

        /// <summary>
        /// Initializes a new instance of the _4PartOfName validation attribute with the default number of characters.
        /// </summary>
        public _4PartOfName() { }

        /// <summary>
        /// Initializes a new instance of the _4PartOfName validation attribute with a specific number of characters.
        /// </summary>
        /// <param name="numberOfCharacters">The maximum number of characters allowed in the validated string.</param>
        public _4PartOfName(int numberOfCharacters)
        {
            NumberOfCharacters = numberOfCharacters;
        }

        /// <summary>
        /// Performs the validation to check if the value contains exactly four parts.
        /// </summary>
        /// <param name="value">The value being validated.</param>
        /// <param name="validationContext">Contextual information about the validation operation.</param>
        /// <returns>A ValidationResult indicating success or failure.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                // Regex to check if the value consists of four parts separated by spaces
                Regex characterRegex = _4partName();
                bool isMatch = characterRegex.IsMatch(value.ToString()!);

                if (!isMatch)
                {
                    // Return a validation failure with a custom error message
                    return new ValidationResult($"THE_FIELD_{validationContext.MemberName!.ToUpper()}_MUST_CONTAIN_FOUR_PARTS");
                }

                // Return success if the validation passes
                return ValidationResult.Success;
            }

            return null; // Return null if the value is null, meaning no validation is needed
        }

        /// <summary>
        /// Generated regex pattern for matching exactly four words.
        /// </summary>
        /// <returns>Regex pattern for four-part name validation.</returns>
        [GeneratedRegex("^([A-Za-z]+)\\s+([A-Za-z]+)\\s+([A-Za-z]+)\\s+([A-Za-z]+)$")]
        private static partial Regex _4partName();
    }
}
