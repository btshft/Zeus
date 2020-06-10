using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zeus.Shared.Exceptions;
using Zeus.Shared.Extensions;

namespace Zeus.Shared.AppFeature.Extensions
{
    public static class AppFeatureEndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapFeature<TFeature>(this IEndpointRouteBuilder endpoints, bool required = true) 
            where TFeature : IAppFeature
        {
            var feature = endpoints.ServiceProvider.GetService<TFeature>();
            if (feature == null)
            {
                if (required)
                    throw new ConfigurationException($"Required feature '{typeof(TFeature).Name}' not registered");

                // Feature not required
                return endpoints;
            }

            feature.Map(endpoints);

            var logger = endpoints.ServiceProvider.GetLogger<TFeature>();
            if (logger != null)
                logger.LogInformation($"Endpoints: Feature '{typeof(TFeature).Name}' endpoint registered");

            return endpoints;
        }
    }
}