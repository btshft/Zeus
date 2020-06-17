using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Zeus.Features.Alerting;
using Zeus.Features.Bot;
using Zeus.Features.HealthCheck.Checks;
using Zeus.Features.HealthCheck.Services;
using Zeus.Shared.AppFeature;
using Zeus.Shared.AppFeature.Extensions;
using Zeus.Shared.Features.Optional;

namespace Zeus.Features.HealthCheck
{
    public class HealthChecksFeature : OptionalFeature<HealthChecksFeatureOptions>
    {
        public HealthChecksFeature(
            IConfiguration configuration, 
            IHostEnvironment environment, 
            IOptions<HealthChecksFeatureOptions> options) : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features)
        {
            var options = Options.Value;

            services.TryAddSingleton<IHealthCheckResponseWriter, SystemJsonHealthCheckResponseWriter>();

            var builder = services.AddHealthChecks();

            features
                .WhenConfigured<AlertingFeature, AlertingFeatureOptions>(o =>
                {
                    var consulOptions = o.Subscriptions.Store.Consul;
                    if (consulOptions != null)
                    {
                        var urlInfo = new UriBuilder(consulOptions.Address);

                        builder.AddConsul(co =>
                            {
                                co.HostName = urlInfo.Host;
                                co.Port = urlInfo.Port;
                            }, name: "consul",
                            options.Consul?.FailureStatus,
                            timeout: options.Consul?.Timeout);
                    }
                })
                .WhenConfigured<BotFeature, BotFeatureOptions>(o =>
                {
                    var registration = new HealthCheckRegistration(
                        name: "telegram",
                        factory: sp => new TelegramHealthCheck(sp.GetRequiredService<ITelegramBotClient>()),
                        failureStatus: options.Telegram.FailureStatus,
                        tags: default,
                        timeout: options.Telegram.Timeout);

                    builder.Add(registration);
                });
        }

        /// <inheritdoc />
        protected override void MapFeature(IEndpointRouteBuilder endpoints)
        {
            var writer = endpoints.ServiceProvider.GetRequiredService<IHealthCheckResponseWriter>();

            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                AllowCachingResponses = Options.Value.AllowCachingResponses,
                ResponseWriter = writer.WriteAsync
            });
        }
    }
}
