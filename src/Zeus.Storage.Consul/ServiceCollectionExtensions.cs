using System;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Storage.Consul
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsulSubscriptions(this IServiceCollection services,
            Action<ConsulOptions> configureOptions)
        {
            services.AddOptions<ConsulOptions>()
                .Configure(configureOptions)
                .ValidateDataAnnotations();

            services.TryAddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ConsulOptions>>().Value;

                return new ConsulClient(cfg =>
                {
                    cfg.Token = options.Token;
                    cfg.Address = options.Address;
                    cfg.WaitTime = options.WaitTime;
                    cfg.Datacenter = options.Datacenter;
                }, options.ConfigureClient, options.ConfigureHandler);
            });

            services.AddSingleton<ISubscriptionsStore, ConsulSubscriptionsStore>();

            return services;
        }
    }
}