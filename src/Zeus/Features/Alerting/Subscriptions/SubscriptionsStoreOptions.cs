namespace Zeus.v2.Features.Alerting.Subscriptions
{
    public class SubscriptionsStoreOptions
    {
        public bool UseInMemoryStore { get; set; }

        public SubscriptionsConsulStoreOptions Consul { get; set; }
    }
}