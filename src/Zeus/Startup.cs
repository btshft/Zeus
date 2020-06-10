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
using Zeus.Features.HealthCheck;
using Zeus.Features.Localization;
using Zeus.Features.Profiling;
using Zeus.Handlers.Webhook;
using Zeus.Shared.AppFeature.Extensions;
using Zeus.Shared.Features.Extensions;

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
                .AddFromConfiguration<HealthChecksFeature, HealthChecksFeatureOptions>("HealthChecks", required: false)
                .AddFromConfiguration<LocalizationFeature, LocalizationFeatureOptions>("Localization")
                .AddFromConfiguration<ApiFeature, ApiFeatureOptions>("Api")
                .AddFromConfiguration<AlertingFeature, AlertingFeatureOptions>("Alerting")
                .AddFromConfiguration<ClientsFeature, ClientsFeatureOptions>("Clients")
                .AddFromConfiguration<ProfilingFeature, ProfilingFeatureOptions>("Profiling", required: false)
                .AddFromConfiguration<BotFeature, BotFeatureOptions>("Bot");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/error");

            app.UseFeature<LocalizationFeature>();
            app.UseFeature<ProfilingFeature>(required: false);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFeature<HealthChecksFeature>(required: false);
                endpoints.MapFeature<ApiFeature>();
            });

            app.UseFeature<ApiFeature>();
        }
    }
}