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
using Zeus.Features.Mediatr;
using Zeus.Features.Metrics;
using Zeus.Features.Profiling;
using Zeus.Features.Swagger;
using Zeus.Middleware;
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
            services.AddFeatures(Environment, Configuration, new SerilogLoggerFactory(Log.Logger))
                .AddFromConfiguration<HealthChecksFeature, HealthChecksFeatureOptions>("HealthChecks", required: false)
                .AddFromConfiguration<MetricsFeature, MetricsFeatureOptions>("Metrics", required: false)
                .AddFromConfiguration<LocalizationFeature, LocalizationFeatureOptions>("Localization")
                .AddFromConfiguration<SwaggerFeature, SwaggerFeatureOptions>("Swagger")
                .AddFromConfiguration<AlertingFeature, AlertingFeatureOptions>("Alerting")
                .AddFromConfiguration<ClientsFeature, ClientsFeatureOptions>("Clients")
                .AddFromConfiguration<ProfilingFeature, ProfilingFeatureOptions>("Profiling", required: false)
                .AddFromConfiguration<BotFeature, BotFeatureOptions>("Bot")
                .Add<ApiFeature>()
                .Add<MediatrFeature>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/error");
            app.UseMiddleware<LogRequestResponseMiddleware>();

            app.UseFeature<LocalizationFeature>();
            app.UseFeature<ProfilingFeature>(required: false);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFeature<HealthChecksFeature>(required: false);
                endpoints.MapFeature<ApiFeature>();
                endpoints.MapFeature<MetricsFeature>(required: false);
            });

            app.UseFeature<SwaggerFeature>();
        }
    }
}