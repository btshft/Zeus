using System;

namespace Zeus.Shared.AppFeature.Internal
{
    // ReSharper disable once UnusedTypeParameter
    internal class AppFeatureSubscription<TFeature, TOptions>
        where TFeature : class, IAppFeature<TOptions>
        where TOptions : class, new()
    {
        private readonly Action<TOptions> _subscriber;

        public AppFeatureSubscription(Action<TOptions> subscriber)
        {
            _subscriber = subscriber;
        }

        public void Notify(TOptions options)
        {
            _subscriber(options);
        }
    }
}