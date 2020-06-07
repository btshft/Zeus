using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.v2.Stores.Default
{
    public class InMemorySubscriptionsStore : ISubscriptionsStore
    {
        private readonly HashSet<AlertsSubscription> _subscriptions;
        private readonly ReaderWriterLockSlim _lock;

        public InMemorySubscriptionsStore()
        {
            _subscriptions = new HashSet<AlertsSubscription>();
            _lock = new ReaderWriterLockSlim();
        }

        public InMemorySubscriptionsStore(IEnumerable<AlertsSubscription> subscriptions)
        {
            _subscriptions = new HashSet<AlertsSubscription>(subscriptions);
            _lock = new ReaderWriterLockSlim();
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<AlertsSubscription>> GetAsync(string channel, CancellationToken cancellation = default)
        {
            if (channel == null) 
                throw new ArgumentNullException(nameof(channel));

            _lock.EnterReadLock();
            try
            {
                return Task.FromResult<IReadOnlyCollection<AlertsSubscription>>(_subscriptions
                    .Where(s => s.Channel == channel)
                    .ToArray());
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<AlertsSubscription>> GetAllAsync(CancellationToken cancellation = default)
        {
            _lock.EnterReadLock();
            try
            {
                var subscriptions = _subscriptions.ToArray();
                return Task.FromResult<IReadOnlyCollection<AlertsSubscription>>(subscriptions);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public Task<AlertsSubscription> GetAsync(long chatId, string channel, CancellationToken cancellation = default)
        {
            if (channel == null) 
                throw new ArgumentNullException(nameof(channel));

            _lock.EnterReadLock();
            try
            {
                var subscription = _subscriptions.FirstOrDefault(s => s.ChatId == chatId && s.Channel == channel);
                return Task.FromResult(subscription);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public Task<bool> ExistsAsync(string channel, long chatId, CancellationToken cancellation = default)
        {
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));

            _lock.EnterReadLock();
            try
            {
                var exists = _subscriptions.Any(s => s.Channel == channel && s.ChatId == chatId);
                return Task.FromResult(exists);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public Task StoreAsync(AlertsSubscription subscription, CancellationToken cancellation = default)
        {
            if (subscription == null) 
                throw new ArgumentNullException(nameof(subscription));

            _lock.EnterUpgradeableReadLock();
            try
            {
                var existingItem = _subscriptions.FirstOrDefault(s => s.Equals(subscription));
                if (existingItem == null)
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        _subscriptions.Add(subscription);
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RemoveAsync(AlertsSubscription subscription, CancellationToken cancellation = default)
        {
            if (subscription == null) 
                throw new ArgumentNullException(nameof(subscription));

            _lock.EnterWriteLock();
            try
            {
                var existing = _subscriptions.FirstOrDefault(s => s.Equals(subscription));
                if (existing != null)
                    _subscriptions.Remove(existing);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return Task.CompletedTask;
        }
    }
}