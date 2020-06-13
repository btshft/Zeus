using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Zeus.Shared.Exceptions;

namespace Zeus.Shared.Validation
{
    public static class DataAnnotationsValidator
    {
        public static ValidationResult Validate(object instance)
        {
            if (instance == null) 
                throw new ArgumentNullException(nameof(instance));

            var context = new ValidationContext(instance);
            var validationResults = new List<ValidationResult>();

            if (Validator.TryValidateObject(instance, context, validationResults, validateAllProperties: true))
                return ValidationResult.Success;

            var flatResults = new List<ValidationResult>();
            Flatten(validationResults, ref flatResults);

            var errors = flatResults.Where(s => !string.IsNullOrEmpty(s.ErrorMessage))
                .Select(s => s.ErrorMessage);

            var errorMessage = string.Join(Environment.NewLine, errors);

            return new ValidationResult(errorMessage);
        }

        public static void EnsureValid(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var result = Validate(instance);
            if (result == ValidationResult.Success)
                return;

            throw new ConfigurationException($"Errors occured while validating object '{instance.GetType().Name}': {result.ErrorMessage}");
        }

        private static void Flatten(IEnumerable<ValidationResult> results, ref List<ValidationResult> output)
        {
            output ??= new List<ValidationResult>();

            foreach (var result in results)
            {
                output.Add(result);

                if (result is CompositeValidationResult composite)
                {
                    Flatten(composite.InnerResults, ref output);
                }
            }
        }
    }
}
