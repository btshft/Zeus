using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.v2.Clients.Callback;
using Zeus.v2.Shared.AppFeature;

namespace Zeus.v2.Features.Clients
{
    public class ClientsFeature : AppFeature<ClientsFeatureOptions>
    {
        public ClientsFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<ClientsFeatureOptions> options) 
            : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        public override void Configure(IServiceCollection services, IAppFeatureCollection features)
        {
            services.AddHttpClient<ICallbackClient, CallbackClient>()
                .ConfigureHttpClient((sp, cl) =>
                {
                    var options = Options.Value;
                    cl.BaseAddress = new Uri(options.Callback.BaseUrl);
                    cl.Timeout = options.Callback.Timeout;
                });
        }

        /// <inheritdoc />
        public override void Use(IApplicationBuilder builder)
        {
        }
    }
}