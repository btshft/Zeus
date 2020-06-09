using Zeus.Shared.Validation;

namespace Zeus.Features.Alerting.Subscriptions
{
    public class SubscriptionsStoreOptions
    {
        public bool UseInMemoryStore { get; set; }

        [ValidateObject]
        public SubscriptionsConsulStoreOptions Consul { get; set; }

        [ValidateObject]
        public SubscriptionsFasterStoreOptions Faster { get; set; }
    }
}