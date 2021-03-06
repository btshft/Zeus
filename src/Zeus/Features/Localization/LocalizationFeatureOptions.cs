﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Zeus.Features.Localization
{
    public class LocalizationFeatureOptions
    {
        [CustomValidation(typeof(LocalizationFeatureOptions), nameof(ValidateCulture))]
        public string Culture { get; set; }

        public static ValidationResult ValidateCulture(string culture, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(culture))
                return new ValidationResult("Culture not set");

            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new CultureInfo(culture);
            }
            catch (Exception e)
            {
                return new ValidationResult($"Unable to create culture with name '{culture}': {e.Message}");
            }

            return ValidationResult.Success;
        }
    }
}