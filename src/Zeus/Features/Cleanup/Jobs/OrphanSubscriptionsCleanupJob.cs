using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zeus.Shared.CronJob;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Features.Cleanup.Jobs
{
    public class OrphanSubscriptionsCleanupJob : ICronJob
    {
        private readonly ISubscriptionsStore _subscriptionsStore;
        private readonly IChannelStore _channelStore;

        private readonly ILogger<OrphanSubscriptionsCleanupJob> _logger;

        public OrphanSubscriptionsCleanupJob(
            ISubscriptionsStore subscriptionsStore, 
            IChannelStore channelStore, 
            ILogger<OrphanSubscriptionsCleanupJob> logger)
        {
            _subscriptionsStore = subscriptionsStore;
            _channelStore = channelStore;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(CancellationToken cancellation = default)
        {
            var subscriptions = await _subscriptionsStore.GetAllAsync(cancellation);
            var channels = await _channelStore.GetAllAsync(cancellation);

            var orphanSubscriptions = new List<AlertsSubscription>();
            foreach (var subscription in subscriptions)
            {
                var hasMatchedChannel = channels.Any(c => string
                    .Equals(c.Name, subscription.Channel, StringComparison.InvariantCultureIgnoreCase));

                if (!hasMatchedChannel)
                    orphanSubscriptions.Add(subscription);
            }

            _logger.LogInformation($"Found '{orphanSubscriptions.Count}' orphan subscriptions");

            foreach (var orphanSubscription in orphanSubscriptions)
            {
                _logger.LogInformation("Removing orphan subscription {Chat} {Channel}", 
                    orphanSubscription.ChatName, orphanSubscription.Channel);

                await _subscriptionsStore.RemoveAsync(orphanSubscription, cancellation);
            }
        }
    }
}