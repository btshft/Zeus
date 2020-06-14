using System;
using System.ComponentModel.DataAnnotations;
using Zeus.Shared.Validation;
using Zeus.Storage.Models.Alerts;

namespace Zeus.Features.Alerting.Channels
{
    public class ChannelsStoreOptions
    {
        [CustomValidation(typeof(ChannelsStoreOptions), nameof(ValidatePredefined))]
        public AlertsChannel[] Predefined { get; set; } = Array.Empty<AlertsChannel>();

        public static ValidationResult ValidatePredefined(AlertsChannel[] channels, ValidationContext context)
        {
            if (channels == null) 
                return ValidationResult.Success;

            foreach (var channel in channels)
            {
                var result = DataAnnotationsValidator.Validate(channel);
                if (result != ValidationResult.Success)
                {
                    var name = channel.Name ?? "<null>";
                    return new ValidationResult($"Channel '{name}' is invalid: {result.ErrorMessage}");
                }
            }

            return ValidationResult.Success;
        }
    }
}