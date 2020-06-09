using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zeus.Storage.Faster.Store.Internal;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Storage.Faster.Store.Subscriptions
{
    internal class FasterSubscriptionsStore : ISubscriptionsStore
    {
        private readonly FasterStore<AlertsSubscriptionKey, AlertsSubscription> _store;

        public FasterSubscriptionsStore(FasterStore<AlertsSubscriptionKey, AlertsSubscription> store)
        {
            _store = store;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<AlertsSubscription>> GetAsync(string channel, CancellationToken cancellation = default)
        {
            if (channel == null) 
                throw new ArgumentNullException(nameof(channel));

            var subscriptions = await _store
                .Where(kv =>
                    kv.Value.Channel.Equals(channel, StringComparison.InvariantCultureIgnoreCase))
                .Select(kv => kv.Value)
                .ToArrayAsync(cancellation);

            return subscriptions;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<AlertsSubscription>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _store.Select(s => s.Value)
                .ToListAsync(cancellation);
        }

        /// <inheritdoc />
        public async Task<AlertsSubscription> GetAsync(long chatId, string channel, CancellationToken cancellation = default)
        {
            var subscriptions = await _store
                .Where(kv =>
                    kv.Value.Channel.Equals(channel, StringComparison.InvariantCultureIgnoreCase) && kv.Value.ChatId == chatId)
                .Select(kv => kv.Value)
                .ToArrayAsync(cancellation);

            return subscriptions.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(string channel, long chatId, CancellationToken cancellation = default)
        {
            return await _store
                .AnyAsync(kv =>
                    kv.Value.Channel.Equals(channel, StringComparison.InvariantCultureIgnoreCase) &&
                    kv.Value.ChatId == chatId, cancellationToken: cancellation);
        }

        /// <inheritdoc />
        public async Task StoreAsync(AlertsSubscription subscription, CancellationToken cancellation = default)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var key = new AlertsSubscriptionKey(subscription.ChatId, subscription.Channel);

            await _store.StoreAsync(key, subscription, cancellation);
            await _store.CommitAsync(cancellation);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(AlertsSubscription subscription, CancellationToken cancellation = default)
        {
            if (subscription == null) 
                throw new ArgumentNullException(nameof(subscription));

            var key = new AlertsSubscriptionKey(subscription.ChatId, subscription.Channel);

            await _store.RemoveAsync(key, cancellation);
            await _store.CommitAsync(cancellation);
        }
    }
}