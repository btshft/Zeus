using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Zeus.Features.Metrics.Core;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Features.Metrics.Collectors
{
    public class ActiveSubscriptionsMetricsCollector : IMetricsCollector
    {
        private readonly ISubscriptionsStore _subscriptionsStore;
        private readonly IMetrics _metrics;

        public ActiveSubscriptionsMetricsCollector(ISubscriptionsStore subscriptionsStore, IMetrics metrics)
        {
            _subscriptionsStore = subscriptionsStore;
            _metrics = metrics;
        }

        /// <inheritdoc />
        public async Task CollectAsync(CancellationToken cancellation = default)
        {
            var subscriptions = await _subscriptionsStore.GetAllAsync(cancellation);

            foreach (var subscriptionGroup in subscriptions.GroupBy(s => s.Channel))
            {
                var tags = new MetricTags("channel", subscriptionGroup.Key);
                var subscriptionsCount = subscriptionGroup.Count();

                _metrics.Measure.Gauge.SetValue(MetricsRegistry.Gauges.ActiveSubscriptions, tags, subscriptionsCount);
            }
        }
    }
}