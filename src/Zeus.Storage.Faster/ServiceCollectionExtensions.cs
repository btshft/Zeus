using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zeus.Storage.Faster.Options;
using Zeus.Storage.Faster.Store.Internal;
using Zeus.Storage.Faster.Store.Subscriptions;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Storage.Faster
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFasterSubscriptions(this IServiceCollection services,
            Action<FasterStoreOptions> configureOptions)
        {
            services.AddOptions<FasterStoreOptions>()
                .Configure(configureOptions)
                .ValidateDataAnnotations();

            services.TryAddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<FasterStoreOptions>>();
                var logger = sp.GetRequiredService<ILogger<FasterStore<AlertsSubscriptionKey, AlertsSubscription>>>();

                return new FasterStore<AlertsSubscriptionKey, AlertsSubscription>(options, 
                    new AlertsSubscriptionKey.Comparer(), logger);
            });

            services.AddSingleton<ISubscriptionsStore, FasterSubscriptionsStore>();

            return services;
        }
    }
}
