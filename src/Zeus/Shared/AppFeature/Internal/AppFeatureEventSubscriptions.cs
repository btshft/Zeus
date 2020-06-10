using System.Collections.Generic;
using System.Linq;

namespace Zeus.Shared.AppFeature.Internal
{
    internal class AppFeatureEventSubscriptions
    {
        private readonly AppFeatureServiceProvider _serviceProvider;

        public AppFeatureEventSubscriptions(AppFeatureServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<AppFeatureSubscription<TFeature, TOptions>> Get<TFeature, TOptions>() 
            where TFeature : class, IAppFeature<TOptions> 
            where TOptions : class, new()
        {
            return _serviceProvider.GetServicesOf(typeof(AppFeatureSubscription<TFeature, TOptions>))
                .Cast<AppFeatureSubscription<TFeature, TOptions>>();
        }

        public void Add<TFeature, TOptions>(AppFeatureSubscription<TFeature, TOptions> subscription)
            where TFeature : class, IAppFeature<TOptions>
            where TOptions : class, new()
        {
            _serviceProvider.AddService(subscription);
        }
    }
}