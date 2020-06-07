﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zeus.v2.Shared.Exceptions;
using Zeus.v2.Shared.Extensions;

namespace Zeus.v2.Shared.AppFeature.Extensions
{
    public static class AppFeatureApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFeature<TFeature>(this IApplicationBuilder builder, bool required = true)
            where TFeature : IAppFeature
        {
            var feature = builder.ApplicationServices.GetService<TFeature>();
            if (feature == null)
            {
                if (required)
                    throw new ConfigurationException($"Required feature '{typeof(TFeature).Name}' not registered");

                // Feature not required
                return builder;
            }

            feature.Use(builder);

            var logger = builder.ApplicationServices.GetLogger<TFeature>();
            if (logger != null)
                logger.LogInformation($"Feature '{typeof(TFeature).Name}' enabled");

            return builder;
        }
    }
}