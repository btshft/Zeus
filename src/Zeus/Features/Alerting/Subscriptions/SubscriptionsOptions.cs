using System.ComponentModel.DataAnnotations;

namespace Zeus.v2.Features.Alerting.Subscriptions
{
    public class SubscriptionsOptions
    {
        [Required]
        public SubscriptionsStoreOptions Store { get; set; }
    }
}