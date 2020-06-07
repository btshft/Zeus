using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Zeus.v2.Shared.AppFeature
{
    public interface IAppFeature
    {
        void Configure(IServiceCollection services, IAppFeatureCollection features);
        void Use(IApplicationBuilder builder);
    }

    public interface IAppFeature<TOptions> : IAppFeature
        where TOptions : class
    { }
}