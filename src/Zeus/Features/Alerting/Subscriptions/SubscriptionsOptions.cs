using System.ComponentModel.DataAnnotations;
using Zeus.Shared.Validation;

namespace Zeus.Features.Alerting.Subscriptions
{
    public class SubscriptionsOptions
    {
        [Required, ValidateObject]
        public SubscriptionsStoreOptions Store { get; set; }
    }
}