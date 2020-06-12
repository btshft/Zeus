using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Zeus.Shared.AppFeature
{
    public interface IAppFeature
    {
        bool Configure(IServiceCollection services, IAppFeatureCollection features);
        bool Use(IApplicationBuilder builder);
        bool Map(IEndpointRouteBuilder endpoints);
    }

    // ReSharper disable once UnusedTypeParameter
    public interface IAppFeature<TOptions> : IAppFeature
        where TOptions : class
    { }
}