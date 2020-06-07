using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zeus.v2.Shared.AppFeature.Internal;

namespace Zeus.v2.Shared.AppFeature.Extensions
{
    public static class AppFeatureServiceCollectionExtensions
    {
        public static IAppFeatureCollection AddFeatures(this IServiceCollection services, 
            IHostEnvironment environment, 
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            return new AppFeatureCollection(services, new AppFeatureServiceProvider(environment, configuration, loggerFactory));
        }
    }
}