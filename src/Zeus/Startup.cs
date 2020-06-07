using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Logging;
using Zeus.v2.Features.Alerting;
using Zeus.v2.Features.Api;
using Zeus.v2.Features.Bot;
using Zeus.v2.Features.Clients;
using Zeus.v2.Features.Profiling;
using Zeus.v2.Handlers.Webhook;
using Zeus.v2.Shared.AppFeature.Extensions;
using Zeus.v2.Shared.Features.Extensions;
using Zeus.v2.Shared.Serilog;

namespace Zeus.v2
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(AlertManagerUpdateRequestHandler));

            services.AddFeatures(Environment, Configuration, new SerilogLoggerFactory(Log.Logger))
                .AddFromConfiguration<ApiFeature, ApiFeatureOptions>("Api")
                .AddFromConfiguration<AlertingFeature, AlertingFeatureOptions>("Alerting")
                .AddFromConfiguration<ClientsFeature, ClientsFeatureOptions>("Clients")
                .AddFromConfiguration<ProfilingFeature, ProfilingFeatureOptions>("Profiling", required: false)
                .AddFromConfiguration<BotFeature, BotFeatureOptions>("Bot");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<EnrichLogContextMiddleware>();

            app.UseFeature<ProfilingFeature>(required: false);
            app.UseFeature<ApiFeature>();
        }
    }
}