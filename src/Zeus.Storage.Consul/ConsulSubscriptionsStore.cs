using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Storage.Consul
{
    /// <summary>
    /// Consul subscriotion store.
    /// </summary>
    internal class ConsulSubscriptionsStore : ISubscriptionsStore
    {
        /// <summary>
        /// 'a//b/c/d///' -> 'a/b/c/d/'
        /// </summary>
        private static readonly Regex SlashReplaceRegex = new Regex(@"(?<slash>[\/])+", RegexOptions.Compiled);

        private readonly ConsulClient _consulClient;
        private readonly IOptions<ConsulOptions> _optionsProvider;

        public ConsulSubscriptionsStore(ConsulClient consulClient, IOptions<ConsulOptions> optionsProvider)
        {
            _consulClient = consulClient;
            _optionsProvider = optionsProvider;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<AlertsSubscription>> GetAsync(string channel,
            CancellationToken cancellation = default)
        {
            var key = GetSubscriptionsKey(channel);
            var subscriptions = await _consulClient.KV.List(key, cancellation).UnwrapAsync();
            if (subscriptions == null)
                return Array.Empty<AlertsSubscription>();

            var resultSubscriptions = new List<AlertsSubscription>(subscriptions.Length);
            foreach (var subscription in subscriptions)
            {
                var jsonContent = Encoding.UTF8.GetString(subscription.Value);
                var subscriptionTyped =
                    JsonConvert.DeserializeObject<AlertsSubscription>(jsonContent,
                        _optionsProvider.Value.SerializerSettings);

                resultSubscriptions.Add(subscriptionTyped);
            }

            return resultSubscriptions;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<AlertsSubscription>> GetAllAsync(CancellationToken cancellation = default)
        {
            var key = GetSubscriptionsKey();
            var subscriptions = await _consulClient.KV.List(key, cancellation).UnwrapAsync();

            var resultSubscriptions = new List<AlertsSubscription>(subscriptions.Length);
            foreach (var subscription in subscriptions)
            {
                var jsonContent = Encoding.UTF8.GetString(subscription.Value);
                var subscriptionTyped =
                    JsonConvert.DeserializeObject<AlertsSubscription>(jsonContent,
                        _optionsProvider.Value.SerializerSettings);

                resultSubscriptions.Add(subscriptionTyped);
            }

            return resultSubscriptions;
        }

        /// <inheritdoc />
        public async Task<AlertsSubscription> GetAsync(long chatId, string channel, CancellationToken cancellation = default)
        {
            var key = GetSubscriptionKey(channel, chatId);
            var result = await _consulClient.KV.List(key, cancellation).UnwrapAsync();
            if (result == null || result.Length == 0) 
                return null;

            var subscriptionBytes = result.First().Value;
            var subscriptionJson = Encoding.UTF8.GetString(subscriptionBytes);

            return JsonConvert.DeserializeObject<AlertsSubscription>(subscriptionJson,
                _optionsProvider.Value.SerializerSettings);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(string channel, long chatId, CancellationToken cancellation = default)
        {
            var key = GetSubscriptionKey(channel, chatId);
            var result = await _consulClient.KV.List(key, cancellation).UnwrapAsync();
            return result != null && result.Length > 0;
        }

        /// <inheritdoc />
        public async Task StoreAsync(AlertsSubscription subscription, CancellationToken cancellation = default)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var key = GetSubscriptionKey(subscription.Channel, subscription.ChatId);

            var jsonString = JsonConvert.SerializeObject(subscription, _optionsProvider.Value.SerializerSettings);
            var utf8Bytes = Encoding.UTF8.GetBytes(jsonString);

            var result = await _consulClient.KV.Put(new KVPair(key)
            {
                Value = utf8Bytes
            }, cancellation);

            if (!result.Response)
                throw new ConsulRequestException(
                    $"Unable to store subscription (chat: {subscription.ChatId}; channel: {subscription.Channel})",
                    result.StatusCode);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(AlertsSubscription subscription, CancellationToken cancellation = default)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var key = GetSubscriptionKey(subscription.Channel, subscription.ChatId);
            await _consulClient.KV.Delete(key, cancellation);
        }

        private string GetSubscriptionKey(string channel, long chatId)
        {
            if (string.IsNullOrWhiteSpace(channel))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(channel));

            var prefix = GetFormatterPrefix(_optionsProvider.Value.Prefix);
            var key = $"{prefix}/subscriptions/{channel}/{chatId}";

            return NormalizeKey(key);
        }

        private string GetSubscriptionsKey(string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(channel));

            var prefix = GetFormatterPrefix(_optionsProvider.Value.Prefix);
            var key = $"{prefix}subscriptions/{channel}";

            return NormalizeKey(key);
        }

        private string GetSubscriptionsKey()
        {
            var prefix = GetFormatterPrefix(_optionsProvider.Value.Prefix);
            var key = $"{prefix}subscriptions";

            return NormalizeKey(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetFormatterPrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return prefix;

            return prefix.EndsWith('/') ? prefix : $"{prefix}/";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string NormalizeKey(string key)
        {
            return SlashReplaceRegex
                .Replace(key, m => m.Groups["slash"].Value)
                .Trim('/')
                .ToLowerInvariant();
        }
    }
}