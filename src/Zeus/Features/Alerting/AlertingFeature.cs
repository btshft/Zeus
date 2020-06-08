using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.Services.Templating;
using Zeus.Services.Templating.Scriban;
using Zeus.Shared.AppFeature;
using Zeus.Shared.Extensions;
using Zeus.Storage.Consul;
using Zeus.Storage.Stores.Abstractions;
using Zeus.Stores.Default;

namespace Zeus.Features.Alerting
{
    public class AlertingFeature : AppFeature<AlertingFeatureOptions>
    {
        public AlertingFeature(
            IConfiguration configuration, 
            IHostEnvironment environment, 
            IOptions<AlertingFeatureOptions> options) 
            : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        public override void Configure(IServiceCollection services, IAppFeatureCollection features)
        {
            var options = Options.Value;

            services.AddOptions<ScribanOptions>()
                .ValidateDataAnnotations();

            services.TryAddSingleton<ITemplateEngine, ScribanTemplateEngine>();
            services.AddSingleton<ITemplatesStore>(
                sp => new FileSystemTemplatesStore(options.Templates.Store.FileSystem.Path));

            if (options.Subscriptions.Store.UseInMemoryStore)
            {
                services.AddSingleton<ISubscriptionsStore, InMemorySubscriptionsStore>();
            }
            else if (options.Subscriptions.Store.Consul != null)
            {
                services.AddConsulSubscriptions(Configuration.CreateBinder<ConsulOptions>(
                    "Alerting:Subscriptions:Store:Consul", required: true));
            }

            if (options.Channels.Store.Predefined != null)
                services.AddSingleton<IChannelStore>(sp => new InMemoryChannelStore(
                    options.Channels.Store.Predefined));
        }

        /// <inheritdoc />
        public override void Use(IApplicationBuilder builder)
        {
        }
    }
}