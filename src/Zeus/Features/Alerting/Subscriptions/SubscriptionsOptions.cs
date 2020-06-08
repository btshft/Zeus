using System.ComponentModel.DataAnnotations;

namespace Zeus.Features.Alerting.Subscriptions
{
    public class SubscriptionsOptions
    {
        [Required]
        public SubscriptionsStoreOptions Store { get; set; }
    }
}