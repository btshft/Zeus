using System.Linq;
using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.Features.Metrics.Collectors;
using Zeus.Shared.AppFeature;

namespace Zeus.Features.Metrics
{
    public class MetricsFeature : AppFeature<MetricsFeatureOptions>
    {
        public MetricsFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<MetricsFeatureOptions> options) 
            : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features)
        {
            services.AddMetrics(s =>
            {
                s.Configuration.Configure(cfg =>
                {
                    cfg.AddEnvTag();
                    cfg.DefaultContextLabel = Options.Value.Prefix;
                });

                s.OutputMetrics.AsPrometheusPlainText();
            });

            services.AddMetricsEndpoints(o =>
            {
                o.MetricsTextEndpointEnabled = false;
                o.MetricsEndpointEnabled = true;

                o.MetricsEndpointOutputFormatter = o.MetricsOutputFormatters
                    .OfType<MetricsPrometheusTextOutputFormatter>()
                    .First();
            });

            services.AddTransient<IMetricsCollector, ActiveSubscriptionsMetricsCollector>();
        }

        /// <inheritdoc />
        protected override void MapFeature(IEndpointRouteBuilder endpoints)
        {
            var metricsApp = endpoints.CreateApplicationBuilder()
                .Use(async (context, next) =>
                {
                    var collectors = context.RequestServices.GetServices<IMetricsCollector>();

                    foreach (var collector in collectors)
                        await collector.CollectAsync(context.RequestAborted);

                    await next();
                })
                .UseMetricsEndpoint();

            endpoints.MapGet("/metrics", metricsApp.Build());
        }
    }
}
