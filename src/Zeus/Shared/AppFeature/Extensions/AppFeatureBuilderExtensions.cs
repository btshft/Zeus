using System;
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
        public static IAppFeatureCollection Add<TFeature, TOptions>(this IAppFeatureCollection builder,
            Action<TOptions> configure) 
            where TFeature : class, IAppFeature<TOptions> 
            where TOptions : class, new()
        {
            var options = (IOptions<TOptions>) new AppFeatureOptions<TOptions>(configure);
            var feature = ActivatorUtilities.CreateInstance<TFeature>(builder.ServiceProvider, options);

            feature.Configure(builder.Services, builder);

            builder.Services.TryAddSingleton(feature);
            builder.Services.AddOptions<TOptions>()
                .Configure(configure)
                .ValidateDataAnnotations();

            var logger = builder.ServiceProvider.GetLogger<TFeature>();
            if (logger != null)
                logger.LogInformation($"Services: Feature '{typeof(TFeature).Name}' registered successfully");

            return builder;
        }

        public static IAppFeatureCollection AddWhen<TFeature, TOptions>(this IAppFeatureCollection builder,
            Func<bool> predicate, Action<TOptions> configure)
            where TFeature : class, IAppFeature<TOptions>
            where TOptions : class, new()
        {
            if (!predicate())
                return builder;

            var options = (IOptions<TOptions>) new AppFeatureOptions<TOptions>(configure);
            var feature = ActivatorUtilities.CreateInstance<TFeature>(builder.ServiceProvider, options);

            feature.Configure(builder.Services, builder);

            builder.Services.TryAddSingleton(feature);
            builder.Services.AddOptions<TOptions>()
                .Configure(configure)
                .ValidateDataAnnotations();

            var logger = builder.ServiceProvider.GetLogger<TFeature>();
            if (logger != null)
                logger.LogInformation($"Pipeline: Feature '{typeof(TFeature).Name}' registered successfully");

            return builder;
        }
    }
}