using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Zeus.Shared.AppFeature
{
    public interface IAppFeature
    {
        void Configure(IServiceCollection services, IAppFeatureCollection features);
        void Use(IApplicationBuilder builder);
        void Map(IEndpointRouteBuilder endpoints);
    }

    public interface IAppFeature<TOptions> : IAppFeature
        where TOptions : class
    { }
}