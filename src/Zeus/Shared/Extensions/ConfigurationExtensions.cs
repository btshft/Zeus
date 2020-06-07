using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Zeus.Shared.Exceptions;

namespace Zeus.Shared.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool SectionExists(this IConfiguration configuration, string sectionName)
        {
            var section = configuration.GetSection(sectionName);
            return section.Exists();
        }

        public static IConfigurationSection GetRequiredSection(this IConfiguration configuration, string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var section = configuration.GetSection(name);
            if (!section.Exists())
                throw new ConfigurationException($"Section '{section}' not exists in configuration");

            return section;
        }

        public static TOptions CreateOptions<TOptions>(this IConfigurationSection section)
            where TOptions : class, new()
        {
            var options = new TOptions();
            section.Bind(options);

            ValidateObject(options);

            return options;
        }

        public static TOptions CreateOptions<TOptions>(this IConfiguration configuration, string name) 
            where TOptions : class, new()
        {
            if (name == null) 
                throw new ArgumentNullException(nameof(name));

            var section = configuration.GetRequiredSection(name);
            var options = new TOptions();

            section.Bind(options);

            ValidateObject(options);

            return options;
        }

        public static Action<TOptions> CreateBinder<TOptions>(this IConfiguration configuration, string name,
            bool required)
        {
            if (name == null) 
                throw new ArgumentNullException(nameof(name));

            return (o) =>
            {
                var section = required ? configuration.GetRequiredSection(name) : configuration.GetSection(name);
                if (section.Exists())
                {
                    section.Bind(o);
                }
            };
        }

        private static void ValidateObject(object instance)
        {
            if (instance == null) 
                throw new ArgumentNullException(nameof(instance));

            var context = new ValidationContext(instance);
            var validationResults = new List<ValidationResult>();

            if (Validator.TryValidateObject(instance, context, validationResults, validateAllProperties: true))
                return;

            var errors = validationResults.Where(s => !string.IsNullOrEmpty(s.ErrorMessage))
                .Select(s => s.ErrorMessage);

            var errorMessage = string.Join(';', errors);
            throw new ConfigurationException($"Errors occured while validating object '{instance.GetType().Name}': {errorMessage}");
        }
    }
}