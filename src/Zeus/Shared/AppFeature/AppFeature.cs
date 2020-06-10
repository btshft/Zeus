using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Zeus.Shared.AppFeature
{
    public abstract class AppFeature<TOptions> : IAppFeature<TOptions>
        where TOptions : class, new()
    {
        protected IConfiguration Configuration { get; }

        protected IOptions<TOptions> Options { get; }

        protected IHostEnvironment Environment { get; }

        protected AppFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<TOptions> options)
        {
            Configuration = configuration;
            Environment = environment;
            Options = options;
        }

        public virtual void Configure(IServiceCollection services, IAppFeatureCollection features)
        { }

        public virtual void Use(IApplicationBuilder builder)
        { }

        public virtual void Map(IEndpointRouteBuilder endpoints)
        { }
    }
}