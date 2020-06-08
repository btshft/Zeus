using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.Shared.AppFeature;
using Zeus.Shared.Security.TrustedNetwork;

namespace Zeus.Features.Api.Authorization
{
    public class AuthorizationFeature : AppFeature<AuthorizationFeatureOptions>
    {
        /// <inheritdoc />
        public AuthorizationFeature(
            IConfiguration configuration,
            IHostEnvironment environment,
            IOptions<AuthorizationFeatureOptions> options)
            : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        public override void Configure(IServiceCollection services, IAppFeatureCollection features)
        {
            var options = Options.Value;
            if (options.TrustedNetwork == null)
                return;

            foreach (var (policy, trustedNetworkOptions) in options.TrustedNetwork)
            {
                services.AddTrustedNetworkFilter(policy, o =>
                {
                    o.Networks = trustedNetworkOptions.Networks;
                });
            }
        }

        /// <inheritdoc />
        public override void Use(IApplicationBuilder builder)
        {
        }
    }
}
