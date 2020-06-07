using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Zeus.v2.Features.Alerting.Templates
{
    public class TemplatesFileSystemStoreOptions
    {
        /// <summary>
        /// Path to templates.
        /// </summary>
        [CustomValidation(typeof(TemplatesFileSystemStoreOptions), nameof(Validate))]
        public string Path { get; set; }

        public static ValidationResult Validate(string path, ValidationContext context)
        {
            if (path == null)
                return new ValidationResult("Path not set");

            if (!Directory.Exists(path))
                return new ValidationResult($"Directory '{path}' not found");

            return ValidationResult.Success;
        }
    }
}