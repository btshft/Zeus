using Microsoft.AspNetCore.Builder;
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

        public abstract void Configure(IServiceCollection services, IAppFeatureCollection features);

        public abstract void Use(IApplicationBuilder builder);
    }
}