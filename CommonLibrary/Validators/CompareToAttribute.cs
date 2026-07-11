using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Custom validation attribute that compares the current property value to another property value
    /// using the specified comparison type (e.g., LessThan, GreaterThan).
    /// </summary>
    public class CompareToAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        private readonly ComparisonType _comparisonType;

        /// <summary>
        /// Initializes a new instance of the CompareToAttribute class.
        /// </summary>
        /// <param name="comparisonProperty">The name of the property to compare against.</param>
        /// <param name="comparisonType">The type of comparison to perform (e.g., LessThan, GreaterThan).</param>
        public CompareToAttribute(string comparisonProperty, ComparisonType comparisonType)
        {
            _comparisonProperty = comparisonProperty;
            _comparisonType = comparisonType;
        }

        /// <summary>
        /// Performs the validation to compare the current value to the specified property value.
        /// </summary>
        /// <param name="value">The current value being validated.</param>
        /// <param name="validationContext">Contextual information about the validation operation.</param>
        /// <returns>A ValidationResult indicating success or failure.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = value as IComparable;

            // Get the property to compare with
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
            {
                throw new ArgumentException("PROPERTY_WITH_THIS_NAME_NOT_FOUND");
            }

            var comparisonValue = property.GetValue(validationContext.ObjectInstance) as IComparable;

            if (currentValue != null && comparisonValue != null)
            {
                bool isValid = false;

                // Perform the comparison based on the specified comparison type
                switch (_comparisonType)
                {
                    case ComparisonType.LessThan:
                        isValid = currentValue.CompareTo(comparisonValue) < 0;
                        break;
                    case ComparisonType.LessThanOrEqual:
                        isValid = currentValue.CompareTo(comparisonValue) <= 0;
                        break;
                    case ComparisonType.GreaterThan:
                        isValid = currentValue.CompareTo(comparisonValue) > 0;
                        break;
                    case ComparisonType.GreaterThanOrEqual:
                        isValid = currentValue.CompareTo(comparisonValue) >= 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // If the comparison fails, return a validation error
                if (!isValid)
                {
                    if (string.IsNullOrEmpty(ErrorMessage))
                    {
                        ErrorMessage = $"THE_VALUE_OF_{validationContext.MemberName.ToUpper()}_MUST_BE_{_comparisonType.ToString().ToUpper()}_THE_VALUE_OF_{_comparisonProperty.ToUpper()}.";
                    }
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Enum representing the different types of comparisons that can be performed.
    /// </summary>
    public enum ComparisonType
    {
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }
}
