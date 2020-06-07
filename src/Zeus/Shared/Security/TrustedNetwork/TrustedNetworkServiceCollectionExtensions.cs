using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Zeus.v2.Shared.Security.TrustedNetwork
{
    public static class TrustedNetworkServiceCollectionExtensions
    {
        public static IServiceCollection AddTrustedNetworkFilter(this IServiceCollection services, string policyName, Action<TrustedNetworkOptions> configure)
        {
            services.AddOptions<TrustedNetworkOptions>(policyName)
                .Configure(configure)
                .ValidateDataAnnotations();

            services.TryAddScoped<AuthorizeTrustedNetworkAttribute>();

            return services;
        }
    }
}