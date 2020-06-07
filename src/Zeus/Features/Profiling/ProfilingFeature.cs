using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Zeus.v2.Features.Profiling.Behaviors;
using Zeus.v2.Features.Profiling.Decorators;
using Zeus.v2.Features.Profiling.Diagnostics;
using Zeus.v2.Shared.AppFeature;
using Zeus.v2.Shared.Diagnostics;

namespace Zeus.v2.Features.Profiling
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
        public override void Configure(IServiceCollection services, IAppFeatureCollection features)
        {
            var options = Options.Value;

            services.AddMemoryCache();
            services.AddMiniProfiler(o =>
            {
                o.RouteBasePath = options.Path;
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ProfilingBehavior<,>));
            services.AddDiagnosticObserver<HttpClientProfilingDiagnosticObserver>();
            services.Decorate<IStringLocalizerFactory, ProfilingStringLocalizerFactory>();
        }

        /// <inheritdoc />
        public override void Use(IApplicationBuilder builder)
        {
            builder.UseMiniProfiler();
            builder.UseDiagnosticObserver<HttpClientProfilingDiagnosticObserver>();
        }
    }
}