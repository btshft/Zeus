using System;
using Microsoft.Extensions.DependencyInjection;

namespace Zeus.v2.Shared.AppFeature.Internal
{
    internal class AppFeatureCollection : IAppFeatureCollection
    {
        public IServiceCollection Services { get; }
        public IServiceProvider ServiceProvider { get; }

        public AppFeatureCollection(IServiceCollection services, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Services = services;
        }
    }
}