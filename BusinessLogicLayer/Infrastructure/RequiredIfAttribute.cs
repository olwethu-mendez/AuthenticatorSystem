using DataAccessLayer.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.Infrastructure
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _dependentProperty;

        public RequiredIfAttribute(string dependentProperty)
        {
            _dependentProperty = dependentProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Get the property object we are depending on
            var containerType = validationContext.ObjectType;
            var dependentPropertyInfo = containerType.GetProperty(_dependentProperty);

            if (dependentPropertyInfo == null)
                return new ValidationResult($"Unknown property: {_dependentProperty}");

            // Get the actual value of that property (e.g., PhoneNumber)
            var dependentValue = dependentPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            // If the dependent property is NOT null/empty, then THIS property is required
            if (dependentValue != null && !string.IsNullOrWhiteSpace(dependentValue.ToString()))
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required.");
                }
            }

            return ValidationResult.Success;
        }
    }
    public class RequiredIfValueIsAttribute : ValidationAttribute
    {
        private readonly string _dependentProperty;
        private readonly object _targetValue;

        public RequiredIfValueIsAttribute(string dependentProperty, object targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Get the property object we are depending on
            var containerType = validationContext.ObjectType;
            var dependentPropertyInfo = containerType.GetProperty(_dependentProperty);

            if (dependentPropertyInfo == null)
                return new ValidationResult($"Unknown property: {_dependentProperty}");

            // Get the actual value of that property (e.g., PhoneNumber)
            var actualDependentValue = dependentPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            // If the dependent property is NOT null/empty, then THIS property is required
            if (actualDependentValue != null && actualDependentValue.ToString() == _targetValue.ToString())
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required when selecting {_targetValue}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
