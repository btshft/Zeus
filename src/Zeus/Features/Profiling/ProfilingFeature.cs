using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Zeus.Features.Profiling.Decorators;
using Zeus.Features.Profiling.Diagnostics;
using Zeus.Shared.AppFeature;
using Zeus.Shared.Diagnostics;

namespace Zeus.Features.Profiling
{
    public class ProfilingFeature : AppFeature<ProfilingFeatureOptions>
    {
        public ProfilingFeature(
            IConfiguration configuration, 
            IHostEnvironment environment, 
            IOptions<ProfilingFeatureOptions> options) 
            : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features)
        {
            var options = Options.Value;

            services.AddMemoryCache();
            services.AddMiniProfiler(o =>
            {
                o.RouteBasePath = options.Path;
            });

            services.AddDiagnosticObserver<HttpClientProfilingDiagnosticObserver>();
            services.Decorate<IStringLocalizerFactory, ProfilingStringLocalizerFactory>();
        }

        /// <inheritdoc />
        protected override void UseFeature(IApplicationBuilder builder)
        {
            builder.UseMiniProfiler();
            builder.UseDiagnosticObserver<HttpClientProfilingDiagnosticObserver>();
        }
    }
}