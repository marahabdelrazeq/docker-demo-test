using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Custom validation attribute that ensures a phone number follows Egyptian format.
    /// Supports both mobile and landline numbers.
    /// Mobile: 10 or 11 digits starting with 01
    /// Landline: 8-10 digits (area code + local number)
    /// </summary>
    public class EgyptPhoneNumberAttribute : ValidationAttribute
    {
        private readonly PhoneNumberType _phoneType;

        public enum PhoneNumberType
        {
            Mobile,
            Landline,
            Any
        }

        /// <summary>
        /// Initializes a new instance of the EgyptPhoneNumberAttribute class.
        /// </summary>
        /// <param name="phoneType">The type of phone number to validate (Mobile, Landline, or Any).</param>
        public EgyptPhoneNumberAttribute(PhoneNumberType phoneType = PhoneNumberType.Any)
        {
            _phoneType = phoneType;
        }

        /// <summary>
        /// Performs the validation to check if the phone number follows Egyptian format.
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

            string phoneNumber = value.ToString()!.Trim();

            // Remove any non-digit characters for validation
            string digitsOnly = Regex.Replace(phoneNumber, @"[^\d]", "");

            switch (_phoneType)
            {
                case PhoneNumberType.Mobile:
                    if (!IsValidEgyptMobile(digitsOnly))
                    {
                        return new ValidationResult("INVALID_EGYPT_MOBILE_FORMAT");
                    }
                    break;

                case PhoneNumberType.Landline:
                    if (!IsValidEgyptLandline(digitsOnly))
                    {
                        return new ValidationResult("INVALID_EGYPT_LANDLINE_FORMAT");
                    }
                    break;

                case PhoneNumberType.Any:
                    if (!IsValidEgyptMobile(digitsOnly) && !IsValidEgyptLandline(digitsOnly))
                    {
                        return new ValidationResult("INVALID_EGYPT_PHONE_FORMAT");
                    }
                    break;
            }

            return ValidationResult.Success!;
        }

        /// <summary>
        /// Validates if the phone number follows Egyptian mobile format.
        /// Egyptian mobile numbers: 10 digits starting with 1
        /// Valid patterns: 10xxxxxxxx, 11xxxxxxxx, 12xxxxxxxx, 15xxxxxxxx
        /// </summary>
        private bool IsValidEgyptMobile(string phoneNumber)
        {
            // Egyptian mobile format: starts with 1[0125], followed by 8 digits
            // Total: 10 digits
            return Regex.IsMatch(phoneNumber, @"^1[0125]\d{8}$");
        }

        /// <summary>
        /// Validates if the phone number follows Egyptian landline format.
        /// Egyptian landline format: area code (1-3 digits) + local number (6-8 digits)
        /// </summary>
        private bool IsValidEgyptLandline(string phoneNumber)
        {
            // Egyptian landline format: 8-10 digits
            // Area codes: 2 (Cairo), 3 (Alexandria), etc.
            // Example: 0223456789 (Cairo), 033456789 (Alexandria)
            return Regex.IsMatch(phoneNumber, @"^0[2-9]\d{7,9}$");
        }
    }
}
