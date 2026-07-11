using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Custom validation attribute that ensures a string or a collection of strings matches a predefined set of allowed values.
    /// </summary>
    public class EnumStringAttribute : ValidationAttribute
    {
        private readonly string[] _allowedValues;

        /// <summary>
        /// Initializes a new instance of the EnumStringAttribute class with a predefined set of allowed string values.
        /// </summary>
        /// <param name="allowedValues">The list of allowed string values.</param>
        public EnumStringAttribute(params string[] allowedValues)
        {
            _allowedValues = allowedValues;
        }

        /// <summary>
        /// Performs the validation to check if the value matches one of the allowed string values.
        /// </summary>
        /// <param name="value">The value being validated (can be a string or IEnumerable of strings).</param>
        /// <param name="validationContext">Contextual information about the validation operation.</param>
        /// <returns>A ValidationResult indicating success or failure.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // If the value is null, consider it valid
            if (value == null)
            {
                return ValidationResult.Success!;
            }

            // Check if the value is a single string
            if (value is string singleValue)
            {
                if (!_allowedValues.Contains(singleValue.ToUpper()))
                {
                    return new ValidationResult($"INVALID_VALUE_FOR_{validationContext.DisplayName.ToUpper()}");
                }
            }
            // Check if the value is an IEnumerable of strings
            else if (value is IEnumerable<string> stringValues)
            {
                var invalidValues = stringValues.Where(val => !_allowedValues.Contains(val.ToUpper())).ToList();
                if (invalidValues.Any())
                {
                    return new ValidationResult($"INVALID_VALUES_FOR_{validationContext.DisplayName.ToUpper()}: {string.Join(", ", invalidValues)}");
                }
            }
            // Handle unsupported types
            else
            {
                return new ValidationResult($"INVALID_VALUE_FOR_{validationContext.DisplayName.ToUpper()}");
            }

            return ValidationResult.Success!;
        }
    }
}
