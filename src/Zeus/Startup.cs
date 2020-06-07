using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Logging;
using Zeus.Features.Alerting;
using Zeus.Features.Api;
using Zeus.Features.Bot;
using Zeus.Features.Clients;
using Zeus.Features.Profiling;
using Zeus.Handlers.Webhook;
using Zeus.Shared.AppFeature.Extensions;
using Zeus.Shared.Features.Extensions;
using Zeus.Shared.Serilog;

namespace Zeus
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