using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zeus.Shared.AppFeature.Internal;
using Zeus.Shared.Extensions;

namespace Zeus.Shared.AppFeature.Extensions
{
    public static class AppFeatureBuilderExtensions
    {
        public static bool IsEnabled<TFeature>(this IAppFeatureCollection builder) 
            where TFeature : IAppFeature
        {
            return builder.Services.Any(s => s.ServiceType == typeof(TFeature));
        }

        public static IAppFeatureCollection Add<TFeature>(this IAppFeatureCollection builder)
            where TFeature : class, IAppFeature
        {
            var feature = ActivatorUtilities.CreateInstance<TFeature>(builder.ServiceProvider);

            var enabled = feature.Configure(builder.Services, builder); 
            builder.Services.TryAddSingleton(feature);

            var logger = builder.ServiceProvider.GetLogger<TFeature>();

            if (enabled)
                logger?.LogInformation($"Services: Feature '{typeof(TFeature).Name}' registered successfully");

            return builder;
        }

        public static IAppFeatureCollection Add<TFeature, TOptions>(this IAppFeatureCollection builder,
            Action<TOptions> configure) 
            where TFeature : class, IAppFeature<TOptions> 
            where TOptions : class, new()
        {
            var options = new AppFeatureOptions<TOptions>(configure);
            var feature = ActivatorUtilities.CreateInstance<TFeature>(builder.ServiceProvider, (IOptions<TOptions>) options);

            var enabled = feature.Configure(builder.Services, builder);

            if (enabled)
            {
                var subscriptions = builder.ServiceProvider.GetRequiredService<AppFeatureEventSubscriptions>();
                foreach (var subscription in subscriptions.Get<TFeature, TOptions>())
                {
                    subscription.Notify(options.Value);
                }
            }

            builder.Services.TryAddSingleton(feature);
            builder.Services.AddOptions<TOptions>()
                .Configure(configure)
                .ValidateDataAnnotations();

            var logger = builder.ServiceProvider.GetLogger<TFeature>();
            if (enabled)
                logger?.LogInformation($"Services: Feature '{typeof(TFeature).Name}' registered successfully");

            return builder;
        }

        public static IAppFeatureCollection AddWhen<TFeature, TOptions>(this IAppFeatureCollection builder,
            Func<bool> predicate, Action<TOptions> configure)
            where TFeature : class, IAppFeature<TOptions>
            where TOptions : class, new()
        {
            if (!predicate())
                return builder;

            var options = new AppFeatureOptions<TOptions>(configure);
            var feature = ActivatorUtilities.CreateInstance<TFeature>(builder.ServiceProvider, (IOptions<TOptions>) options);
            var enabled = feature.Configure(builder.Services, builder);

            if (enabled)
            {
                var subscriptions = builder.ServiceProvider.GetRequiredService<AppFeatureEventSubscriptions>();
                foreach (var subscription in subscriptions.Get<TFeature, TOptions>())
                {
                    subscription.Notify(options.Value);
                }
            }

            builder.Services.TryAddSingleton(feature);
            builder.Services.AddOptions<TOptions>()
                .Configure(configure)
                .ValidateDataAnnotations();

            var logger = builder.ServiceProvider.GetLogger<TFeature>();
            if (enabled)
                logger?.LogInformation($"Pipeline: Feature '{typeof(TFeature).Name}' registered successfully");

            return builder;
        }

        public static IAppFeatureCollection WhenConfigured<TFeature, TOptions>(this IAppFeatureCollection features,
            Action<TOptions> callback)
            where TFeature : class, IAppFeature<TOptions>
            where TOptions : class, new()
        {
            var subscriptions = features.ServiceProvider.GetRequiredService<AppFeatureEventSubscriptions>();
            subscriptions.Add(new AppFeatureSubscription<TFeature, TOptions>(callback));

            return features;
        }
    }
}