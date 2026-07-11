using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Custom validation attribute for Egyptian vehicle plate codes.
    /// Validates that the plate code contains 2-3 Arabic characters with spaces allowed between them.
    /// Excludes specific prohibited Arabic characters: ث، ح، خ، ز، ذ، ش، ض، ظ، غ، ك، ت
    /// </summary>
    public class EgyptPlateCodeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Performs the validation to check if the value is a valid Egyptian plate code.
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

            string plateCode = value.ToString()!.Trim();

            // Remove spaces to count actual characters
            string plateCodeWithoutSpaces = plateCode.Replace(" ", "");

            // Check if it contains 2-3 characters (excluding spaces)
            if (plateCodeWithoutSpaces.Length < 2 || plateCodeWithoutSpaces.Length > 3)
            {
                return new ValidationResult("PLATE_CODE_MUST_BE_2_TO_3_CHARACTERS");
            }

            // Regex pattern for Egyptian plate codes:
            // - Allows Arabic characters from ء to ي (Arabic alphabet)
            // - EXCLUDES prohibited characters: ث ح خ ز ذ ش ض ظ غ ك ت
            // - Allows spaces between characters
            // Allowed characters: ء أ ؤ إ ئ ا ب ة ج د ر س ص ط ع ف ق ل م ن ه و ى ي
            // Pattern: first char + optional space + 1-2 more chars with optional spaces
            Regex plateCodeRegex = new Regex(@"^[ءأؤإئابةجدرسصطعفقلمنهويى](\s?[ءأؤإئابةجدرسصطعفقلمنهويى]){1,2}$");

            if (!plateCodeRegex.IsMatch(plateCode))
            {
                return new ValidationResult("INVALID_PLATE_CODE_FORMAT_OR_PROHIBITED_CHARACTERS");
            }

            return ValidationResult.Success;
        }
    }
}
