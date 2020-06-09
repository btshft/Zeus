using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Zeus.Shared.Exceptions;

namespace Zeus.Shared.Validation
{
    public static class DataAnnotationsValidator
    {
        public static void EnsureValid(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var results = new List<ValidationResult>();
            if (TryValidateRecursive(instance, ref results))
                return;

            var errors = results.Where(s => !string.IsNullOrEmpty(s.ErrorMessage))
                .Select(s => s.ErrorMessage);

            var errorMessage = string.Join(';', errors);
            throw new ConfigurationException($"Errors occured while validating object '{instance.GetType().Name}': {errorMessage}");
        }

        private static bool TryValidateRecursive(object instance, ref List<ValidationResult> results)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (results == null)
                results = new List<ValidationResult>();

            var validationContext = new ValidationContext(instance);
            var isValid = Validator.TryValidateObject(instance, validationContext, results, validateAllProperties: true);

            var properties = instance.GetType()
                .GetProperties()
                .Where(prop => prop.CanRead && prop.GetIndexParameters().Length == 0)
                .Where(prop => CanValidate(prop.PropertyType))
                .ToArray();

            foreach (var property in properties)
            {
                var value = property.GetValue(instance);
                if (value == null)
                    continue;

                var enumerable = value as IEnumerable<object> ?? new[] { value };

                foreach (var toValidate in enumerable)
                {
                    var nestedResults = new List<ValidationResult>();
                    if (TryValidateRecursive(toValidate, ref nestedResults))
                    {
                        continue;
                    }

                    isValid = false;

                    results.AddRange(nestedResults
                        .Select(result => new ValidationResult(
                            result.ErrorMessage, result.MemberNames
                                .Select(x => property.Name + '.' + x))));
                }
            }


            return isValid;
        }

        /// <summary>
        /// Returns whether the given <paramref name="type"/> can be validated
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanValidate(Type type)
        {
            while (true)
            {
                if (type == null) 
                    return false;

                if (type == typeof(string))
                    return false;

                if (type.IsValueType) 
                    return false;

                if (!type.IsArray || !type.HasElementType) 
                    return true;

                var elementType = type.GetElementType();
                type = elementType;
            }
        }
    }
}
