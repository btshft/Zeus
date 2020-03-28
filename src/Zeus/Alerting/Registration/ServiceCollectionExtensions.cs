using System;
using Microsoft.Extensions.DependencyInjection;
using Zeus.Alerting.Options;
using Zeus.Alerting.Routing;
using Zeus.Alerting.Templating;

namespace Zeus.Alerting.Registration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAlerting(this IServiceCollection services, Action<AlertingOptions> configure)
        {
            services.AddOptions<AlertingOptions>()
                .Configure(configure);

            services.AddSingleton<IAlertRoutesResolver, AlertRoutesResolver>();
            services.AddSingleton<ITemplateResolver, TemplateResolver>();

            return services;
        }
    }
}
