using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.Clients.Callback;
using Zeus.Shared.AppFeature;

namespace Zeus.Features.Clients
{
    public class ClientsFeature : AppFeature<ClientsFeatureOptions>
    {
        public ClientsFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<ClientsFeatureOptions> options) 
            : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features)
        {
            services.AddHttpClient<ICallbackClient, CallbackClient>()
                .ConfigureHttpClient((sp, cl) =>
                {
                    var options = Options.Value;
                    cl.BaseAddress = new Uri(options.Callback.BaseUrl);
                    cl.Timeout = options.Callback.Timeout;
                });
        }
    }
}