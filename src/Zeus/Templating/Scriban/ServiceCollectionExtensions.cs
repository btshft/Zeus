using System;
using Microsoft.Extensions.DependencyInjection;
using Zeus.Templating.Abstraction;

namespace Zeus.Templating.Scriban
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScriban(this IServiceCollection services, Action<ScribanOptions> configureOptions)
        {
            services.AddOptions<ScribanOptions>()
                .Configure(configureOptions)
                .ValidateDataAnnotations();

            services.AddSingleton<ITemplateEngine, ScribanTemplateEngine>();

            return services;
        }
    }
}