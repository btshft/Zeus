using System;
using System.ComponentModel.DataAnnotations;
using Cronos;

namespace Zeus.Shared.CronJob
{
    public class CronJobOptions
    {
        [CustomValidation(typeof(CronJobOptions), nameof(ValidateCron))]
        public string Schedule { get; set; }

        public bool UseLocalTimeZone { get; set; }

        public static ValidationResult ValidateCron(string schedule, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(schedule))
                return new ValidationResult("Schedule not set");

            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                CronExpression.Parse(schedule);
            }
            catch (Exception e)
            {
                return new ValidationResult($"Unable to create schedule '{schedule}': {e.Message}");
            }

            return ValidationResult.Success;
        }
    }
}