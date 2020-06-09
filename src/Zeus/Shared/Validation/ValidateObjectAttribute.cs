using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zeus.Shared.Validation
{
    public class ValidateObjectAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var results = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);

            var isValid = Validator.TryValidateObject(value, context, results, true);
            if (isValid) 
                return ValidationResult.Success;

            var compositeResults =
                new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed");

            compositeResults.InnerResults.AddRange(results);

            return compositeResults;
        }
    }
}