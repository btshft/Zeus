using System;
using Microsoft.Extensions.DependencyInjection;

namespace Zeus.v2.Shared.AppFeature
{
    public interface IAppFeatureCollection
    {
        public IServiceCollection Services { get; }
        public IServiceProvider ServiceProvider { get; }
    }
}